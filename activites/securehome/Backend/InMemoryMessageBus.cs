using Backend.Protocol;
using Microsoft.Extensions.Logging;

namespace Backend;

/// <summary>
/// Simple in-memory message bus implementation using publish/subscribe pattern
/// Mimics the structure of UdpCommunicator but uses in-RAM dictionary
/// </summary>
public class InMemoryMessageBus : IMessageBus
{
    private static readonly Dictionary<string, List<Action<Envelope>>> _globalSubscribers = new();
    private static readonly object _lock = new object();

    private readonly string _nodeId;
    private readonly ILogger _logger;
    private Action<Envelope>? _messageHandler;

    public InMemoryMessageBus(ILoggerFactory loggerFactory, string nodeId)
    {
        _logger = loggerFactory.CreateLogger($"SecureHome MessageBus {nodeId}");
        _nodeId = nodeId;
    }

    public void Start()
    {
        if (_messageHandler == null)
        {
            _logger.LogWarning("No callback registered for incoming messages");
        }

        // Subscribe this node to receive messages
        lock (_lock)
        {
            if (!_globalSubscribers.ContainsKey(_nodeId))
            {
                _globalSubscribers[_nodeId] = new List<Action<Envelope>>();
            }

            if (_messageHandler != null)
            {
                _globalSubscribers[_nodeId].Add(_messageHandler);
            }
        }

        _logger.LogInformation("InMemory MessageBus started for node {NodeId}", _nodeId);
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_globalSubscribers.ContainsKey(_nodeId))
            {
                _globalSubscribers[_nodeId].Clear();
            }
        }

        _logger.LogInformation("MessageBus stopped for node {NodeId}", _nodeId);
    }

    public void Send(Envelope envelope, string? topic = null)
    {
        lock (_lock)
        {
            // Broadcast to all subscribers
            if (envelope.RecipientId == Envelope.RecipientBroadcast)
            {
                foreach (var nodeHandlers in _globalSubscribers.Values)
                {
                    foreach (var handler in nodeHandlers)
                    {
                        // Don't send to self for broadcasts
                        if (!_globalSubscribers[_nodeId].Contains(handler))
                        {
                            Task.Run(() => handler(envelope));
                        }
                    }
                }
                _logger.LogInformation("Broadcast message {Type} from {Sender}", envelope.Type, envelope.SenderId);
            }
            // Unicast to specific recipient
            else if (_globalSubscribers.ContainsKey(envelope.RecipientId))
            {
                foreach (var handler in _globalSubscribers[envelope.RecipientId])
                {
                    Task.Run(() => handler(envelope));
                }
                _logger.LogInformation("Unicast message {Type} from {Sender} to {Recipient}",
                    envelope.Type, envelope.SenderId, envelope.RecipientId);
            }
            else
            {
                _logger.LogWarning("Recipient {Recipient} not found", envelope.RecipientId);
            }
        }
    }

    public Action<Envelope>? OnMessageReceived
    {
        private get => _messageHandler;
        set => _messageHandler = value;
    }

    public override string ToString()
    {
        return $"InMemoryMessageBus {_nodeId}";
    }
}
