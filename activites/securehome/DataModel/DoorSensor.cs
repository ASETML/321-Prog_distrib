namespace DataModel;

/// <summary>
/// Sensor that detects door opening/forcing
/// </summary>
public class DoorSensor : SecurityProbe
{
    public DoorSensor(string probeId, string location) : base(probeId, location)
    {
    }

    public override bool CheckThreat(ThreatEnvironment threat)
    {
        // This sensor triggers on door-related threats at its location
        return IsActive &&
               threat.ThreatType == ThreatType.DOOR_FORCED &&
               threat.TargetLocation.Equals(Location, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetProbeType()
    {
        return "Door Sensor";
    }
}
