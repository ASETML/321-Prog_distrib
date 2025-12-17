using Backend;
using Backend.Protocol;
using DataModel;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Windows.Forms;

namespace ConsoleUI;

/// <summary>
/// Console application for house security management
/// Houses can install sensors and respond to alerts and verification requests
/// </summary>
public class HouseSecurityConsole
{
    private House _house;
    private SecurityAgent _agent;
    private ILoggerFactory _loggerFactory;
    private ThreatEnvironment? _currentThreat;
    private Dictionary<string, SecurityAlert> _sentAlerts = new();
    private Action<string> _writeOutput;

    public HouseSecurityConsole(ILoggerFactory loggerFactory, Action<string> writeOutput)
    {
        _loggerFactory = loggerFactory;
        _writeOutput = writeOutput;

        // TODO 01: Créer votre maison et ajouter des capteurs de sécurité
        // IMPORTANT: Use numeric ID without spaces (e.g., "house001", "house002")
        // Example: _house = new House("house001", "Villa Sunrise");
        //          _house.AddProbe(new DoorSensor("DS001", "Front Door"));
        //          _house.AddProbe(new WindowSensor("WS001", "Living Room Window"));
        _house = new House("house001", "ASETML House");
        _house.AddProbe(new WindowSensor("probe001", "Living Room Window"));
        _house.AddProbe(new WindowSensor("probe002", "Garage"));
        _house.AddProbe(new WindowSensor("probe003", "Hallway"));

        _house.AddProbe(new DoorSensor("probe004", "Front Door"));
        _house.AddProbe(new DoorSensor("probe005", "Back Door"));
        _house.AddProbe(new DoorSensor("probe006", "Garage"));

        _house.AddProbe(new GlassBreakDetector("probe007", "Kitchen Window"));

        _house.AddProbe(new MotionDetector("probe008", "Bedroom Window"));

        // Create agent
        var messageBus = new InMemoryMessageBus(_loggerFactory, _house.UniqueName);
        messageBus.OnMessageReceived = HandleMessage;
        _agent = new SecurityAgent(_house.UniqueName, messageBus, _loggerFactory);
    }

    public void Start()
    {
        _agent.Start();

        // Display startup information
        _writeOutput("═══════════════════════════════════════");
        _writeOutput($"Welcome to {_house.DisplayName}!");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("TODO 01: You must customize your house first!");
        _writeOutput("");
        _writeOutput("Edit HouseSecurityConsole.cs to:");
        _writeOutput("  - Set house ID (numeric, no spaces: house001, house002...)");
        _writeOutput("  - Set house display name");
        _writeOutput("  - Add security probes (DoorSensor, WindowSensor, MotionDetector)");
        _writeOutput("");
        _writeOutput($"Current House ID: {_house.UniqueName}");
        _writeOutput($"(Use this ID for verify/arm/disarm commands)");
        _writeOutput($"Status: {(_house.IsArmed ? "ARMED" : "DISARMED")}");
        _writeOutput("");
        _writeOutput("Installed Probes:");
        foreach (var probe in _house.GetProbes())
        {
            _writeOutput($"  {probe}");
        }
        _writeOutput("");
        _writeOutput("Available Commands:");
        _writeOutput("  arm       - Arm security system");
        _writeOutput("  disarm    - Disarm security system");
        _writeOutput("  status    - Show current status");
        _writeOutput("  probes    - List all probes");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("");
    }


    public void HandleUserCommand(string command)
    {
        switch (command.ToLower())
        {
            case "arm":
                _house.SetArmed(true);
                _writeOutput("Security system ARMED");
                break;

            case "disarm":
                _house.SetArmed(false);
                _writeOutput("Security system DISARMED");
                break;

            case "status":
                DisplayStatus();
                break;

            case "probes":
                DisplayProbes();
                break;

            default:
                _writeOutput("Unknown command. Try: arm, disarm, status, probes");
                break;
        }
    }

    private void HandleMessage(Envelope envelope)
    {
        // Ignore messages from self
        if (envelope.SenderId == _house.UniqueName)
            return;

        switch (envelope.Type)
        {
            case MessageType.THREAT_ENVIRONMENT:
                HandleThreatEvent(envelope);
                break;


            // TODO 08: Désérialiser la demande de vérification et réagir en conséquence...

            case MessageType.THREAT_CHECK:
                DialogResult result = MessageBox.Show($"{envelope.Message}", "Is this a real threat ?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SendVerificationResponse("id", envelope.Message, true, "");
                }
                else
                {
                    SendVerificationResponse("id", envelope.Message, false, "");
                } 
                break;

            // TODO 13: Gérer une commande à distance

        }
    }

    private void HandleThreatEvent(Envelope envelope)
    {
        var threat = ThreatEnvironment.FromJson(envelope.Message);
        if (threat == null || threat.ThreatType == ThreatType.NONE)
            return;

        _currentThreat = threat;

        // Check if any sensor detects this threat
        foreach (var probe in _house.GetProbes())
        {
            if (probe.CheckThreat(threat))
            {
                probe.IsTriggered = true;
                _writeOutput("");
                _writeOutput($"ALERT! {probe.GetProbeType()} at '{probe.Location}' triggered!");
                _writeOutput($"    Threat: {threat.ThreatType} (Severity: {threat.SeverityLevel})");

                // TODO 03: Créer et envoyer une alerte au MonitoringCenter
                _agent.SendEnvelope(new Envelope(this._house.UniqueName, MessageType.THREAT, JsonSerializer.Serialize(threat), "monitoring_center"));
                probe.IsTriggered = false;
            }
        }
    }

    private void SendVerificationResponse(string requestId, string alertId, bool isRealThreat, string comment)
    {
        // TODO 09: Envoyer une réponse de vérification au MonitoringCenter
    }

    private void SendCommandResult(string commandId, CommandStatus status, string message)
    {
        // TODO 14: Envoyer le résultat de la commande au MonitoringCenter
    }

    private void DisplayStatus()
    {
        _writeOutput("");
        _writeOutput($"House: {_house.DisplayName} ({_house.UniqueName})");
        _writeOutput($"Status: {(_house.IsArmed ? "ARMED" : "DISARMED")}");
        _writeOutput($"Probes: {_house.GetProbes().Count}");
        _writeOutput($"Active Probes: {_house.GetProbes().Count(p => p.IsActive)}");
        _writeOutput("");
    }

    private void DisplayProbes()
    {
        _writeOutput("");
        _writeOutput("Installed Probes:");
        foreach (var probe in _house.GetProbes())
        {
            _writeOutput($"  {probe}");
        }
        _writeOutput("");
    }
}
