namespace DataModel;

/// <summary>
/// Base class for all security probes
/// </summary>
public abstract class SecurityProbe
{
    public string ProbeId { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }
    public bool IsTriggered { get; set; }

    protected SecurityProbe(string probeId, string location)
    {
        ProbeId = probeId;
        Location = location;
        IsActive = false;  // Probes start inactive (house starts disarmed)
        IsTriggered = false;
    }

    /// <summary>
    /// Check if this probe should trigger based on the threat
    /// </summary>
    public abstract bool CheckThreat(ThreatEnvironment threat);

    /// <summary>
    /// Get the probe type name
    /// </summary>
    public abstract string GetProbeType();

    public override string ToString()
    {
        return $"{GetProbeType()} at {Location} ({ProbeId}) - {(IsActive ? "Active" : "Inactive")}";
    }
}
