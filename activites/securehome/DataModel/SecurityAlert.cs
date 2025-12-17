using System.Text.Json;

namespace DataModel;

/// <summary>
/// Alert sent from house to monitoring center when a sensor is triggered
/// </summary>
public class SecurityAlert
{
    public string AlertId { get; init; } = Guid.NewGuid().ToString();
    public string HouseId { get; set; }
    public string ProbeId { get; set; }
    public string Location { get; set; }
    public DateTime Timestamp { get; set; }
    public string ThreatDescription { get; set; }
    public int SeverityLevel { get; set; }
    public bool Verified { get; set; } = false;
    public bool IsRealThreat { get; set; } = false;

    public SecurityAlert(string houseId, string probeId, string location, string threatDescription, int severityLevel)
    {
        HouseId = houseId;
        ProbeId = probeId;
        Location = location;
        Timestamp = DateTime.Now;
        ThreatDescription = threatDescription;
        SeverityLevel = severityLevel;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static SecurityAlert? FromJson(string json)
    {
        return JsonSerializer.Deserialize<SecurityAlert>(json);
    }

    public override string ToString()
    {
        return $"Alert {AlertId}: {ThreatDescription} at {Location} (Severity: {SeverityLevel})";
    }
}
