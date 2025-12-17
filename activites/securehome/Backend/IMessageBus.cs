using Backend.Protocol;

namespace Backend;

/// <summary>
/// Interface for message bus communication (mimics ICommunicator)
/// </summary>
public interface IMessageBus
{
    void Send(Envelope envelope, string? topic = null);

    Action<Envelope>? OnMessageReceived { set; }

    void Start();

    void Stop();
}
