using System.Text.Json;

namespace DataModel;

/// <summary>
/// Result status of command execution
/// </summary>
public enum CommandStatus
{
    EXECUTED,
    REFUSED_BY_USER,
    FAILED
}

/// <summary>
/// Result of command execution sent back to monitoring center
/// </summary>
public class CommandResult
{
    public string CommandId { get; set; }
    public CommandStatus Status { get; set; }
    public string Message { get; set; }
    public DateTime ExecutedAt { get; set; }

    public CommandResult(string commandId, CommandStatus status, string message)
    {
        CommandId = commandId;
        Status = status;
        Message = message;
        ExecutedAt = DateTime.Now;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static CommandResult? FromJson(string json)
    {
        return JsonSerializer.Deserialize<CommandResult>(json);
    }

    public override string ToString()
    {
        return $"Command {CommandId} - {Status}: {Message}";
    }
}
