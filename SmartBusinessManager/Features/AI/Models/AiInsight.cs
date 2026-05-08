using System.Text.Json.Serialization;

namespace SmartBusinessManager.Features.AI.Models;

public class AiInsight
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("insight_type")]
    public string InsightType { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("is_read")]
    public bool IsRead { get; set; }

    [JsonPropertyName("generated_at")]
    public DateTime GeneratedAt { get; set; }
}
