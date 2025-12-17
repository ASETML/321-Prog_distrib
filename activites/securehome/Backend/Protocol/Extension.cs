namespace Backend.Protocol;

public static class Extension
{
    public static Envelope Create(this MessageType type, string senderId, string message, string recipientId = Envelope.RecipientBroadcast)
    {
        return new Envelope(senderId, type, message, recipientId);
    }
}
