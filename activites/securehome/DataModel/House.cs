namespace DataModel;

/// <summary>
/// Represents a house with security probes
/// </summary>
public class House
{
    /// <summary>
    /// Technical identifier (used in messages)
    /// </summary>
    public string UniqueName { get; private set; }

    /// <summary>
    /// Display name for the user
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Security system state
    /// </summary>
    public bool IsArmed { get; set; }

    /// <summary>
    /// List of installed security probes
    /// </summary>
    private List<SecurityProbe> _probes;

    public House(string uniqueName, string displayName)
    {
        UniqueName = uniqueName;
        DisplayName = displayName;
        IsArmed = false;
        _probes = new List<SecurityProbe>();
    }

    /// <summary>
    /// Add a security probe to the house
    /// </summary>
    public void AddProbe(SecurityProbe probe)
    {
        _probes.Add(probe);
    }

    /// <summary>
    /// Get all installed probes
    /// </summary>
    public List<SecurityProbe> GetProbes()
    {
        return _probes;
    }

    /// <summary>
    /// Arm or disarm the security system
    /// </summary>
    public void SetArmed(bool armed)
    {
        IsArmed = armed;
        foreach (var probe in _probes)
        {
            probe.IsActive = armed;
        }
    }

    public override string ToString()
    {
        return $"{DisplayName} ({UniqueName}) - {(IsArmed ? "ARMED" : "DISARMED")} - {_probes.Count} probes";
    }
}
