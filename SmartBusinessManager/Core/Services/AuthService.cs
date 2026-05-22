using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly SessionManager _sessionManager;

    public AuthService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        _http = new HttpClient();
    }

    public async Task<ApiResponse<AppUser>> SignInAsync(string email, string password)
    {
        try
        {
            var url = $"{Constants.AuthBase}/token?grant_type=password";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("apikey", Constants.SupabaseAnonKey);

            var requestBody = new
            {
                email = email,
                password = password
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(resp);
            }

            var responseJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            string accessToken = root.GetProperty("access_token").GetString();
            string refreshToken = root.GetProperty("refresh_token").GetString();
            var userElement = root.GetProperty("user");
            string userId = userElement.GetProperty("id").GetString();
            string userEmail = userElement.GetProperty("email").GetString();

            // Save session keys to SecureStorage
            await _sessionManager.SaveSessionAsync(accessToken, refreshToken, userId);

            var appUser = new AppUser
            {
                Id = userId,
                Email = userEmail
            };

            // Attempt to retrieve profile from profiles table
            try
            {
                var profile = await FetchProfileAsync(userId, accessToken);
                if (profile != null)
                {
                    appUser.FullName = profile.FullName;
                    appUser.BusinessName = profile.BusinessName;
                    appUser.AvatarUrl = profile.AvatarUrl;
                }
                else
                {
                    appUser.FullName = email.Split('@')[0];
                }
            }
            catch
            {
                // Fallback to name extracted from email
                appUser.FullName = email.Split('@')[0];
            }

            return ApiResponse<AppUser>.Success(appUser);
        }
        catch (Exception ex)
        {
            return ApiResponse<AppUser>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<AppUser>> SignUpAsync(string email, string password, string fullName)
    {
        try
        {
            var url = $"{Constants.AuthBase}/signup";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("apikey", Constants.SupabaseAnonKey);

            var requestBody = new
            {
                email = email,
                password = password,
                options = new
                {
                    data = new
                    {
                        full_name = fullName
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(resp);
            }

            var responseJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var userElement = root.GetProperty("user");
            string userId = userElement.GetProperty("id").GetString();
            string userEmail = userElement.GetProperty("email").GetString();

            string accessToken = null;
            string refreshToken = null;

            if (root.TryGetProperty("access_token", out var accessProp) && accessProp.ValueKind != JsonValueKind.Null)
            {
                accessToken = accessProp.GetString();
            }
            if (root.TryGetProperty("refresh_token", out var refreshProp) && refreshProp.ValueKind != JsonValueKind.Null)
            {
                refreshToken = refreshProp.GetString();
            }

            // Insert new profile record using Rest API (uses the anon key or token)
            await InsertProfileAsync(userId, userEmail, fullName, accessToken ?? Constants.SupabaseAnonKey);

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await _sessionManager.SaveSessionAsync(accessToken, refreshToken, userId);
            }

            var appUser = new AppUser
            {
                Id = userId,
                Email = userEmail,
                FullName = fullName
            };

            return ApiResponse<AppUser>.Success(appUser);
        }
        catch (Exception ex)
        {
            return ApiResponse<AppUser>.Failure(ex.Message);
        }
    }

    public Task SignOutAsync()
    {
        _sessionManager.ClearSession();
        return Task.CompletedTask;
    }

    private async Task<ProfileDto> FetchProfileAsync(string userId, string token)
    {
        var url = $"{Constants.RestBase}/profiles?id=eq.{userId}&select=*";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("apikey", Constants.SupabaseAnonKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _http.SendAsync(request);
        if (!resp.IsSuccessStatusCode) return null;

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var p = root[0];
            return new ProfileDto
            {
                FullName = p.TryGetProperty("full_name", out var fn) ? fn.GetString() : null,
                BusinessName = p.TryGetProperty("business_name", out var bn) ? bn.GetString() : null,
                AvatarUrl = p.TryGetProperty("avatar_url", out var au) ? au.GetString() : null
            };
        }

        return null;
    }

    private async Task InsertProfileAsync(string userId, string email, string fullName, string token)
    {
        var url = $"{Constants.RestBase}/profiles";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", Constants.SupabaseAnonKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Prefer", "return=minimal");

        var requestBody = new
        {
            id = userId,
            email = email,
            full_name = fullName
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(request);
        if (!resp.IsSuccessStatusCode)
        {
            var errorContent = await resp.Content.ReadAsStringAsync();
            string errMsg = null;
            try
            {
                using var doc = JsonDocument.Parse(errorContent);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    errMsg = msgProp.GetString();
                }
            }
            catch { }

            throw new Exception(errMsg ?? $"Failed to create user profile: {resp.StatusCode}");
        }
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage resp)
    {
        var errorContent = await resp.Content.ReadAsStringAsync();
        string errMsg = null;
        try
        {
            using var doc = JsonDocument.Parse(errorContent);
            var root = doc.RootElement;
            if (root.TryGetProperty("error_description", out var descProp))
            {
                errMsg = descProp.GetString();
            }
            else if (root.TryGetProperty("msg", out var msgProp))
            {
                errMsg = msgProp.GetString();
            }
            else if (root.TryGetProperty("error", out var errProp))
            {
                errMsg = errProp.GetString();
            }
        }
        catch { }

        if (string.IsNullOrEmpty(errMsg))
        {
            errMsg = $"Authentication failed with code: {resp.StatusCode}";
        }

        throw new Exception(errMsg);
    }

    private class ProfileDto
    {
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
