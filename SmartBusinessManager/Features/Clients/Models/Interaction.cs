using System.Text.Json.Serialization;

namespace SmartBusinessManager.Features.Clients.Models;

public class Interaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } // call / meeting / message / note

    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("interacted_at")]
    public DateTime InteractedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
