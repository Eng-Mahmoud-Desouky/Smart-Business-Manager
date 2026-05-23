using System.Net.Http.Headers;
using System.Text;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class AiService : IAiService
{
    private readonly HttpClient _http;
    private readonly SessionManager _sessionManager;

    public AiService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        _http = new HttpClient();
    }

    public async Task<bool> GenerateInsightsAsync(string ownerId)
    {
        var url = $"{Constants.RestBase}/rpc/generate_ai_insights";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", Constants.SupabaseAnonKey);

        var token = _sessionManager.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Send an empty body for the RPC call
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
