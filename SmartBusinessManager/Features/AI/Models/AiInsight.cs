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

    [JsonIgnore]
    public string DisplayTitle => InsightType switch
    {
        "overdue_payment" => "Overdue Payment Alert",
        "payment_due_soon" => "Upcoming Payment Reminder",
        "follow_up_needed" => "Follow-Up Suggested",
        "inactive_client" => "Inactive Client Re-engagement",
        "high_value_client" => "High-Value Client Opportunity",
        _ => "AI Smart Recommendation"
    };

    [JsonIgnore]
    public string DisplayAction => InsightType switch
    {
        "overdue_payment" => "Review Payment",
        "payment_due_soon" => "Send Invoice",
        "follow_up_needed" => "Log Contact",
        "inactive_client" => "Reach Out",
        "high_value_client" => "View CRM Feed",
        _ => "Take Action"
    };

    [JsonIgnore]
    public string DisplayTime => GeneratedAt.ToLocalTime().ToString("MMM dd, yyyy");
}
