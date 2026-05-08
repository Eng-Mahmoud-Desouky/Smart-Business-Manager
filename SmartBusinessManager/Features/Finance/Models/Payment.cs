using System.Text.Json.Serialization;

namespace SmartBusinessManager.Features.Finance.Models;

public class Payment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "USD";

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("due_date")]
    public DateOnly? DueDate { get; set; }

    [JsonPropertyName("paid_at")]
    public DateTime? PaidAt { get; set; }

    [JsonPropertyName("reference_number")]
    public string ReferenceNumber { get; set; }
}
