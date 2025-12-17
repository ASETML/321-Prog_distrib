using Backend;
using Backend.Protocol;
using DataModel;
using Microsoft.Extensions.Logging;

namespace ConsoleUI;

/// <summary>
/// Console application for security monitoring center
/// Receives alerts from houses and can send verification requests and commands
/// </summary>
public class MonitoringCenterConsole
{
    private const string CENTER_ID = "monitoring_center";
    private SecurityAgent _agent;
    private ILoggerFactory _loggerFactory;
    private Dictionary<string, SecurityAlert> _activeAlerts = new();
    private List<string> _registeredHouses = new();
    private Action<string> _writeOutput;

    public MonitoringCenterConsole(ILoggerFactory loggerFactory, Action<string> writeOutput)
    {
        _loggerFactory = loggerFactory;
        _writeOutput = writeOutput;

        // Create agent
        var messageBus = new InMemoryMessageBus(_loggerFactory, CENTER_ID);
        messageBus.OnMessageReceived = HandleMessage;
        _agent = new SecurityAgent(CENTER_ID, messageBus, _loggerFactory);
    }

    public void Start()
    {
        _agent.Start();

        // Display startup information
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("Welcome to Monitoring Center!");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("Status: Waiting for alerts from houses...");
        _writeOutput("");
        _writeOutput("Available Commands:");
        _writeOutput("  alerts                       - Show active alerts");
        _writeOutput("  verify <house> <alertId>     - Request verification");
        _writeOutput("    Example: verify house001 A-12345");
        _writeOutput("  arm <house>                  - Send ARM command");
        _writeOutput("    Example: arm house001");
        _writeOutput("  disarm <house>               - Send DISARM command");
        _writeOutput("    Example: disarm house001");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("");
    }


    public void HandleOperatorCommand(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return;

        switch (parts[0].ToLower())
        {
            case "alerts":
                DisplayAlerts();
                break;

            case "verify":
                if (parts.Length >= 3)
                {
                    string houseId = parts[1];
                    string alertId = parts[2];
                    // TODO 07: Envoyer une demande de vérification à la maison
                    _agent.SendMessage(MessageType.THREAT_CHECK, alertId, houseId);
                }
                else
                {
                    _writeOutput("Usage: verify <house_id> <alert_id>");
                }
                break;

            case "arm":
                if (parts.Length >= 2)
                {
                    string houseId = parts[1];
                    // TODO 12: Envoyer une commande ARM_SYSTEM à la maison
                }
                else
                {
                    _writeOutput("Usage: arm <house_id>");
                }
                break;

            case "disarm":
                if (parts.Length >= 2)
                {
                    string houseId = parts[1];
                    // TODO 12: Envoyer une commande DISARM_SYSTEM à la maison
                }
                else
                {
                    _writeOutput("Usage: disarm <house_id>");
                }
                break;

            default:
                _writeOutput("Unknown command");
                break;
        }
    }

    private void HandleMessage(Envelope envelope)
    {
        switch (envelope.Type)
        {
            case MessageType.HELLO:
                if (!_registeredHouses.Contains(envelope.SenderId))
                {
                    _registeredHouses.Add(envelope.SenderId);
                    _writeOutput($"House '{envelope.SenderId}' connected");
                }
                break;


            // TODO 04: Recevoir et afficher les alertes de sécurité des maisons

            case MessageType.THREAT:
                ThreatEnvironment threat = ThreatEnvironment.FromJson(envelope.Message);
                _writeOutput($"House {envelope.SenderId} threat {threat.ThreatType} of severity {threat.SeverityLevel} at: {threat.TargetLocation}");

                // TODO 05: Envoyer une confirmation de réception d'alerte

                _agent.SendMessage(MessageType.THREAT_RECEIVED, envelope.Message, envelope.SenderId);
                break;
            // TODO 10: Recevoir et afficher les réponses de vérification

            // TODO 15: Recevoir et afficher les résultats de commandes
        }
    }

    private void DisplayAlerts()
    {
        _writeOutput("");
        if (_activeAlerts.Count == 0)
        {
            _writeOutput("No active alerts");
        }
        else
        {
            _writeOutput($"Active Alerts ({_activeAlerts.Count}):");
            foreach (var alert in _activeAlerts.Values)
            {
                _writeOutput($"  [{alert.Timestamp:HH:mm:ss}] {alert.AlertId}: {alert.ThreatDescription}");
                _writeOutput($"    House: {alert.HouseId}, Location: {alert.Location}");
                _writeOutput($"    Verified: {alert.Verified}, Real Threat: {alert.IsRealThreat}");
            }
        }
        _writeOutput("");
    }
}
