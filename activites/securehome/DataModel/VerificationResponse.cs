using System.Text.Json;

namespace DataModel;

/// <summary>
/// Response from house owner confirming or denying an alert
/// </summary>
public class VerificationResponse
{
    public string RequestId { get; set; }
    public string AlertId { get; set; }
    public bool IsRealThreat { get; set; }
    public string UserComment { get; set; }
    public DateTime RespondedAt { get; set; }

    public VerificationResponse(string requestId, string alertId, bool isRealThreat, string userComment = "")
    {
        RequestId = requestId;
        AlertId = alertId;
        IsRealThreat = isRealThreat;
        UserComment = userComment;
        RespondedAt = DateTime.Now;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static VerificationResponse? FromJson(string json)
    {
        return JsonSerializer.Deserialize<VerificationResponse>(json);
    }

    public override string ToString()
    {
        return $"Verification Response: Alert {AlertId} - {(IsRealThreat ? "REAL THREAT" : "FALSE ALARM")}";
    }
}
