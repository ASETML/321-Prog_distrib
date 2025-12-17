using System.Text.Json;

namespace Backend.Protocol;

public class Envelope(string senderId, MessageType type, string message, string recipientId = Envelope.RecipientBroadcast, bool idempotent = false)
{
    public bool Idempotent { get; init; } = idempotent;

    public const string RecipientBroadcast = "ALL";
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string SenderId { get; init; } = senderId;

    public string? RecipientId { get; init; } = recipientId;

    public MessageType Type { get; init; } = type;

    public string Message { get; init; } = message;

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Envelope? FromJson(string json)
    {
        return JsonSerializer.Deserialize<Envelope>(json);
    }

    public override string ToString()
    {
        return ToJson();
    }
}
