namespace DataModel;

/// <summary>
/// Detector that senses glass breaking sound
/// </summary>
public class GlassBreakDetector : SecurityProbe
{
    public GlassBreakDetector(string probeId, string location) : base(probeId, location)
    {
    }

    public override bool CheckThreat(ThreatEnvironment threat)
    {
        // This sensor triggers on glass break at its location
        return IsActive &&
               threat.ThreatType == ThreatType.GLASS_BREAK &&
               threat.TargetLocation.Equals(Location, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetProbeType()
    {
        return "Glass Break Detector";
    }
}
