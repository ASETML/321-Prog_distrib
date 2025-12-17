namespace DataModel;

/// <summary>
/// Detector that senses motion in a room
/// </summary>
public class MotionDetector : SecurityProbe
{
    public MotionDetector(string probeId, string location) : base(probeId, location)
    {
    }

    public override bool CheckThreat(ThreatEnvironment threat)
    {
        // This sensor triggers on motion detection at its location
        return IsActive &&
               threat.ThreatType == ThreatType.MOTION_DETECTED &&
               threat.TargetLocation.Equals(Location, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetProbeType()
    {
        return "Motion Detector";
    }
}
