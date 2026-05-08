using System.Text.Json.Serialization;

namespace SmartBusinessManager.Features.Clients.Models;

public class Client
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("last_contacted_at")]
    public DateTime? LastContactedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
