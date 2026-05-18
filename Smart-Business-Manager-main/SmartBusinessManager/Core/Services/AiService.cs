using System.Net.Http;
using System.Text;
using System.Text.Json;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class AiService : IAiService
{
    private readonly SessionManager _sessionManager;
    private static readonly HttpClient _httpClient = new HttpClient();

    public AiService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task<bool> GenerateInsightsAsync(string ownerId)
    {
        if (string.IsNullOrEmpty(ownerId))
            return false;

        try
        {
            var url = Constants.EdgeFunctionGenerateInsights;
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("apikey", Constants.SupabaseAnonKey);

            var token = _sessionManager.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }

            var payload = new { owner_id = ownerId };
            var jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"AI Edge Function Error in GenerateInsightsAsync: {response.StatusCode} - {error}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in GenerateInsightsAsync: {ex.Message}");
            return false;
        }
    }
}
