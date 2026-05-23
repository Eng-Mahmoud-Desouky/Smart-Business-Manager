using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;
using SmartBusinessManager.Features.AI.Models;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class SupabaseService : ISupabaseService
{
    private readonly HttpClient _http;
    private readonly SessionManager _sessionManager;

    public SupabaseService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        _http = new HttpClient();
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("apikey", Constants.SupabaseAnonKey);

        var token = await _sessionManager.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    private async Task EnsureSuccessResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            string errMsg = null;
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    errMsg = msgProp.GetString();
                }
            }
            catch { }

            throw new Exception(errMsg ?? $"Database operation failed with status code: {response.StatusCode}");
        }
    }

    // ── Clients CRUD ──────────────────────────────────────────────────────────

    public async Task<List<Client>> GetClientsAsync()
    {
        var url = $"{Constants.RestBase}/clients?select=*&order=created_at.desc";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Client>>(json, options) ?? new List<Client>();
    }

    public async Task<Client> GetClientByIdAsync(string clientId)
    {
        var url = $"{Constants.RestBase}/clients?id=eq.{clientId}&select=*";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        var clients = JsonSerializer.Deserialize<List<Client>>(json);
        return clients != null && clients.Count > 0 ? clients[0] : null;
    }

    public async Task AddClientAsync(Client client)
    {
        var url = $"{Constants.RestBase}/clients";
        var request = await CreateRequestAsync(HttpMethod.Post, url);
        request.Headers.Add("Prefer", "return=minimal");

        // Set RLS owner ID securely using secure async session storage lookup
        client.OwnerId = await _sessionManager.GetCurrentUserIdAsync();

        var json = JsonSerializer.Serialize(client);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            return;
        }
        await EnsureSuccessResponseAsync(response);
    }

    public async Task UpdateClientAsync(Client client)
    {
        var url = $"{Constants.RestBase}/clients?id=eq.{client.Id}";
        var request = await CreateRequestAsync(HttpMethod.Patch, url);

        var json = JsonSerializer.Serialize(client);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);
    }

    public async Task DeleteClientAsync(string clientId)
    {
        var url = $"{Constants.RestBase}/clients?id=eq.{clientId}";
        var request = await CreateRequestAsync(HttpMethod.Delete, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);
    }

    // ── Payments CRUD ─────────────────────────────────────────────────────────

    public async Task<List<Payment>> GetPaymentsAsync()
    {
        var url = $"{Constants.RestBase}/payments?select=*&order=created_at.desc";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Payment>>(json, options) ?? new List<Payment>();
    }

    public async Task<List<Payment>> GetPaymentsByClientAsync(string clientId)
    {
        var url = $"{Constants.RestBase}/payments?client_id=eq.{clientId}&select=*&order=created_at.desc";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Payment>>(json) ?? new List<Payment>();
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        var url = $"{Constants.RestBase}/payments";
        var request = await CreateRequestAsync(HttpMethod.Post, url);
        request.Headers.Add("Prefer", "return=minimal");

        // Set RLS owner ID securely using secure async session storage lookup
        payment.OwnerId = await _sessionManager.GetCurrentUserIdAsync();
        payment.Currency = "USD"; // Fixed to USD for MVP

        var json = JsonSerializer.Serialize(payment);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            return;
        }
        await EnsureSuccessResponseAsync(response);
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        var url = $"{Constants.RestBase}/payments?id=eq.{payment.Id}";
        var request = await CreateRequestAsync(HttpMethod.Patch, url);

        var json = JsonSerializer.Serialize(payment);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);
    }

    // ── AI Insights CRUD ──────────────────────────────────────────────────────

    public async Task<List<AiInsight>> GetInsightsAsync()
    {
        var url = $"{Constants.RestBase}/ai_insights?select=*&order=priority.asc";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AiInsight>>(json) ?? new List<AiInsight>();
    }

    public async Task MarkInsightAsReadAsync(string insightId)
    {
        var url = $"{Constants.RestBase}/ai_insights?id=eq.{insightId}";
        var request = await CreateRequestAsync(HttpMethod.Patch, url);

        var updateBody = new { is_read = true };
        var json = JsonSerializer.Serialize(updateBody);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);
    }

    // ── Interactions Timeline CRUD ────────────────────────────────────────────

    public async Task<List<Interaction>> GetInteractionsByClientAsync(string clientId)
    {
        var url = $"{Constants.RestBase}/interactions?client_id=eq.{clientId}&select=*&order=interacted_at.desc";
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        await EnsureSuccessResponseAsync(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Interaction>>(json) ?? new List<Interaction>();
    }

    public async Task AddInteractionAsync(Interaction interaction)
    {
        var url = $"{Constants.RestBase}/interactions";
        var request = await CreateRequestAsync(HttpMethod.Post, url);
        request.Headers.Add("Prefer", "return=minimal");

        // Set RLS owner ID securely using secure async session storage lookup
        interaction.OwnerId = await _sessionManager.GetCurrentUserIdAsync();

        var json = JsonSerializer.Serialize(interaction);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            return;
        }
        await EnsureSuccessResponseAsync(response);
    }
}
