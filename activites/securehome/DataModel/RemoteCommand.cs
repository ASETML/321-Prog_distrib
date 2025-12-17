using System.Text.Json;

namespace DataModel;

/// <summary>
/// Command types that can be sent from monitoring center to houses
/// </summary>
public enum CommandType
{
    ARM_SYSTEM,
    DISARM_SYSTEM,
    LOCK_DOORS,
    ACTIVATE_SIREN
}

/// <summary>
/// Command sent from monitoring center to house
/// </summary>
public class RemoteCommand
{
    public string CommandId { get; init; } = Guid.NewGuid().ToString();
    public CommandType CommandType { get; set; }
    public string Reason { get; set; }
    public DateTime SentAt { get; set; }

    public RemoteCommand(CommandType commandType, string reason)
    {
        CommandType = commandType;
        Reason = reason;
        SentAt = DateTime.Now;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static RemoteCommand? FromJson(string json)
    {
        return JsonSerializer.Deserialize<RemoteCommand>(json);
    }

    public override string ToString()
    {
        return $"Remote Command {CommandId}: {CommandType} - {Reason}";
    }
}
