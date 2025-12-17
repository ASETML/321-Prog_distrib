using Backend.Protocol;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Backend;

/// <summary>
/// Central message broker that routes messages between components
/// Runs on localhost:12000 and accepts connections from houses, monitoring center, and simulator
/// </summary>
public class MessageBroker
{
    private const int BROKER_PORT = 12000;
    private readonly ILogger _logger;
    private TcpListener? _listener;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
    private readonly ConcurrentDictionary<string, NetworkStream> _streams = new();

    public MessageBroker(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("MessageBroker");
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Loopback, BROKER_PORT);
        _listener.Start();

        _logger.LogInformation("Message Broker started on localhost:{Port}", BROKER_PORT);
        _logger.LogInformation("Waiting for components to connect...");

        // Accept connections in background
        Task.Run(() => AcceptClients(_cancellationTokenSource.Token));
    }

    private async Task AcceptClients(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var client = await _listener!.AcceptTcpClientAsync(token);
                _ = Task.Run(() => HandleClient(client, token), token);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting clients");
        }
    }

    private async Task HandleClient(TcpClient client, CancellationToken token)
    {
        var stream = client.GetStream();
        var buffer = new byte[8192];
        var messageBuffer = new StringBuilder();
        string? clientId = null;

        try
        {
            while (!token.IsCancellationRequested && client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                if (bytesRead == 0)
                {
                    _logger.LogInformation("Client {ClientId} disconnected", clientId ?? "unknown");
                    break;
                }

                var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                messageBuffer.Append(data);

                // Process complete messages (delimited by newline)
                string bufferContent = messageBuffer.ToString();
                var messages = bufferContent.Split('\n');

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    var message = messages[i].Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        clientId = await ProcessMessage(message, client, stream, clientId);
                    }
                }

                // Keep incomplete message in buffer
                messageBuffer.Clear();
                messageBuffer.Append(messages[messages.Length - 1]);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error handling client {ClientId}", clientId ?? "unknown");
        }
        finally
        {
            if (clientId != null)
            {
                _clients.TryRemove(clientId, out _);
                _streams.TryRemove(clientId, out _);
                _logger.LogInformation("Client {ClientId} removed", clientId);
            }
            stream.Close();
            client.Close();
        }
    }

    private async Task<string?> ProcessMessage(string message, TcpClient client, NetworkStream stream, string? currentClientId)
    {
        // Handle registration
        if (message.StartsWith("REGISTER|"))
        {
            var nodeId = message.Substring(9);
            _clients[nodeId] = client;
            _streams[nodeId] = stream;

            var response = $"REGISTERED|{nodeId}\n";
            var data = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(data, 0, data.Length);

            _logger.LogInformation("Client registered: {NodeId} (Total: {Count})", nodeId, _clients.Count);
            return nodeId;
        }

        // Handle envelope messages
        try
        {
            var envelope = Envelope.FromJson(message);
            if (envelope != null)
            {
                await RouteMessage(envelope);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing envelope");
        }

        return currentClientId;
    }

    private async Task RouteMessage(Envelope envelope)
    {
        var messageData = Encoding.UTF8.GetBytes(envelope.ToJson() + "\n");

        // Broadcast to all
        if (envelope.RecipientId == Envelope.RecipientBroadcast)
        {
            _logger.LogDebug("Broadcasting {Type} from {Sender} to {Count} clients",
                envelope.Type, envelope.SenderId, _streams.Count);

            foreach (var kvp in _streams)
            {
                // Don't send back to sender
                if (kvp.Key == envelope.SenderId)
                    continue;

                try
                {
                    await kvp.Value.WriteAsync(messageData, 0, messageData.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error broadcasting to {ClientId}", kvp.Key);
                }
            }
        }
        // Unicast to specific recipient
        else if (_streams.TryGetValue(envelope.RecipientId, out var targetStream))
        {
            _logger.LogDebug("Routing {Type} from {Sender} to {Recipient}",
                envelope.Type, envelope.SenderId, envelope.RecipientId);

            try
            {
                await targetStream.WriteAsync(messageData, 0, messageData.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending to {ClientId}", envelope.RecipientId);
            }
        }
        else
        {
            _logger.LogWarning("Recipient {RecipientId} not found", envelope.RecipientId);
        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _listener?.Stop();

        foreach (var client in _clients.Values)
        {
            client.Close();
        }

        _clients.Clear();
        _streams.Clear();

        _logger.LogInformation("Message Broker stopped");
    }
}
