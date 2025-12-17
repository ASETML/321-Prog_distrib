using Backend;
using Backend.Protocol;
using DataModel;
using Microsoft.Extensions.Logging;

namespace ConsoleUI;

/// <summary>
/// Threat simulator that generates random intrusion events
/// Mimics MotherNature from the original system
/// </summary>
public class ThreatSimulatorConsole
{
    private const string SIMULATOR_ID = "threat_simulator";
    private SecurityAgent _agent;
    private ILoggerFactory _loggerFactory;
    private Random _random = new Random();
    private System.Threading.Timer? _timer;
    private Action<string> _writeOutput;

    private readonly string[] _locations = new[]
    {
        "Front Door",
        "Back Door",
        "Living Room Window",
        "Bedroom Window",
        "Kitchen Window",
        "Hallway",
        "Living Room",
        "Garage Door"
    };

    private readonly ThreatType[] _threatTypes = new[]
    {
        ThreatType.DOOR_FORCED,
        ThreatType.WINDOW_BROKEN,
        ThreatType.MOTION_DETECTED,
        ThreatType.GLASS_BREAK,
        ThreatType.NONE
    };

    public ThreatSimulatorConsole(ILoggerFactory loggerFactory, Action<string> writeOutput)
    {
        _loggerFactory = loggerFactory;
        _writeOutput = writeOutput;

        // Create agent
        var messageBus = new InMemoryMessageBus(_loggerFactory, SIMULATOR_ID);
        _agent = new SecurityAgent(SIMULATOR_ID, messageBus, _loggerFactory);
    }

    public void Start()
    {
        _agent.Start();

        // Display startup information
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("Welcome to Threat Simulator!");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("Status: AUTO MODE - Generating threats every 30 seconds");
        _writeOutput("");
        _writeOutput("Available Commands:");
        _writeOutput("  trigger          - Generate a threat event now");
        _writeOutput("  auto <seconds>   - Change auto-interval");
        _writeOutput("  stop             - Stop auto-generation");
        _writeOutput("═══════════════════════════════════════");
        _writeOutput("");

        // Send initial time sync
        SendTimeSync();

        // Start auto-generation (every 30 seconds by default)
        StartAutoGeneration(30);
    }

    public void HandleCommand(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (parts[0].ToLower())
        {
            case "trigger":
                GenerateAndBroadcastThreat();
                break;

            case "auto":
                if (parts.Length >= 2 && int.TryParse(parts[1], out int seconds))
                {
                    StartAutoGeneration(seconds);
                    _writeOutput($"Auto-generation started (every {seconds} seconds)");
                }
                else
                {
                    _writeOutput("Usage: auto <seconds>");
                }
                break;

            case "stop":
                StopAutoGeneration();
                _writeOutput("Auto-generation stopped");
                break;

            default:
                _writeOutput("Unknown command. Try: trigger, auto <seconds>, stop");
                break;
        }
    }

    private void StartAutoGeneration(int intervalSeconds)
    {
        _timer?.Dispose();
        _timer = new System.Threading.Timer(_ => GenerateAndBroadcastThreat(),
            null,
            TimeSpan.FromSeconds(5),  // First trigger after 5 seconds
            TimeSpan.FromSeconds(intervalSeconds));
    }

    private void StopAutoGeneration()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void GenerateAndBroadcastThreat()
    {
        // 70% chance of a threat, 30% chance of no threat
        var threatType = _random.Next(100) < 70 ? _threatTypes[_random.Next(_threatTypes.Length - 1)] : ThreatType.NONE;

        var location = _locations[_random.Next(_locations.Length)];
        var severity = _random.Next(1, 6);

        var threat = new ThreatEnvironment(
            DateTime.Now,
            threatType,
            location,
            severity
        );

        // Broadcast threat event
        var envelope = MessageType.THREAT_ENVIRONMENT.Create(
            SIMULATOR_ID,
            threat.ToJson(),
            Envelope.RecipientBroadcast
        );

        _agent.SendEnvelope(envelope);

        if (threatType != ThreatType.NONE)
        {
            _writeOutput($"Threat generated: {threatType} at {location} (Severity: {severity})");
        }
    }

    private void SendTimeSync()
    {
        var envelope = MessageType.TIME_SYNC.Create(
            SIMULATOR_ID,
            DateTime.Now.ToString("O"),
            Envelope.RecipientBroadcast
        );

        _agent.SendEnvelope(envelope);
    }
}
