namespace DataModel;

/// <summary>
/// Sensor that detects window opening/breaking
/// </summary>
public class WindowSensor : SecurityProbe
{
    public WindowSensor(string probeId, string location) : base(probeId, location)
    {
    }

    public override bool CheckThreat(ThreatEnvironment threat)
    {
        // This sensor triggers on window-related threats at its location
        return IsActive &&
               (threat.ThreatType == ThreatType.WINDOW_BROKEN || threat.ThreatType == ThreatType.GLASS_BREAK) &&
               threat.TargetLocation.Equals(Location, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetProbeType()
    {
        return "Window Sensor";
    }
}
