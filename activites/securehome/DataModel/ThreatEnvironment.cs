using System.Text.Json;

namespace DataModel;

/// <summary>
/// Types of security threats
/// </summary>
public enum ThreatType
{
    NONE,
    DOOR_FORCED,
    WINDOW_BROKEN,
    MOTION_DETECTED,
    GLASS_BREAK
}

/// <summary>
/// Represents a threat event broadcast by ThreatSimulator
/// Mimics TownEnvironment from the original system
/// </summary>
public class ThreatEnvironment
{
    public DateTime DateTime { get; set; }
    public ThreatType ThreatType { get; set; }
    public string TargetLocation { get; set; }
    public int SeverityLevel { get; set; }  // 1-5

    public ThreatEnvironment(DateTime dateTime, ThreatType threatType, string targetLocation, int severityLevel)
    {
        DateTime = dateTime;
        ThreatType = threatType;
        TargetLocation = targetLocation;
        SeverityLevel = Math.Clamp(severityLevel, 1, 5);
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static ThreatEnvironment? FromJson(string json)
    {
        return JsonSerializer.Deserialize<ThreatEnvironment>(json);
    }

    public override string ToString()
    {
        return $"[{DateTime:HH:mm:ss}] Threat: {ThreatType} at {TargetLocation} (Severity: {SeverityLevel})";
    }
}
