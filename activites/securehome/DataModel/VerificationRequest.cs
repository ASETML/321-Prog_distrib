using System.Text.Json;

namespace DataModel;

/// <summary>
/// Request from monitoring center asking house to verify an alert
/// </summary>
public class VerificationRequest
{
    public string RequestId { get; init; } = Guid.NewGuid().ToString();
    public string AlertId { get; set; }
    public string Question { get; set; }
    public DateTime SentAt { get; set; }

    public VerificationRequest(string alertId, string question)
    {
        AlertId = alertId;
        Question = question;
        SentAt = DateTime.Now;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static VerificationRequest? FromJson(string json)
    {
        return JsonSerializer.Deserialize<VerificationRequest>(json);
    }

    public override string ToString()
    {
        return $"Verification Request {RequestId}: {Question}";
    }
}
