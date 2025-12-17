using Backend.Protocol;
using Microsoft.Extensions.Logging;

namespace Backend;

/// <summary>
/// Security agent that handles message communication
/// Mimics the Agent.cs structure from the original system
/// </summary>
public class SecurityAgent
{
    private readonly IMessageBus _communicator;
    private readonly string _agentId;
    private readonly ILogger _logger;

    public SecurityAgent(string agentId, IMessageBus communicator, ILoggerFactory loggerFactory)
    {
        _agentId = agentId;
        _communicator = communicator;
        _logger = loggerFactory.CreateLogger($"SecurityAgent {agentId}");
        // IMPORTANT: messageBus.OnMessageReceived must be set BEFORE creating SecurityAgent
        // Each console class (HouseSecurityConsole, MonitoringCenterConsole, etc.) sets its own
        // message handler on the messageBus before passing it to this constructor
    }

    /// <summary>
    /// Send a message to a specific recipient or broadcast
    /// </summary>
    public void SendMessage(MessageType type, string message, string? recipientId = null)
    {
        var envelope = new Envelope(
            _agentId,
            type,
            message,
            recipientId ?? Envelope.RecipientBroadcast
        );

        _communicator.Send(envelope);
        _logger.LogDebug("Sent {Type} to {Recipient}", type, recipientId ?? "ALL");
    }

    /// <summary>
    /// Send a pre-constructed envelope
    /// </summary>
    public void SendEnvelope(Envelope envelope)
    {
        _communicator.Send(envelope);
        _logger.LogDebug("Sent envelope {Type}", envelope.Type);
    }

    public void Start()
    {
        _communicator.Start();
        _logger.LogInformation("SecurityAgent {AgentId} started", _agentId);
    }

    public void Stop()
    {
        _communicator.Stop();
        _logger.LogInformation("SecurityAgent {AgentId} stopped", _agentId);
    }

    public string AgentId => _agentId;
}
