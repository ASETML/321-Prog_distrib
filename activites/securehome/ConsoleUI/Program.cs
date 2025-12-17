using Microsoft.Extensions.Logging;
using System.Windows.Forms;

namespace ConsoleUI;

// TODO Généraux obligatoires, pour assurer les points faciles
// - Respecter les conventions de nommage C# (PascalCase, camelCase)
// - Commenter le code de manière appropriée
// - Tester régulièrement votre code
// - Au moins 1 commit par TODO
//
// TODO supplémentaires à choix, pour compenser des TODOs imparfaits (jusqu'à 6 points max)
// - Ajouter un nouveau type de capteur (SmokeDetector, CameraDetector, etc.)
// - Améliorer l'interface console avec des couleurs et meilleure mise en forme
// - Ajouter un système de logs dans des fichiers
// - Implémenter un historique des alertes dans MonitoringCenter
// - Ajouter des statistiques (nombre d'alertes par maison, taux de fausses alarmes, etc.)

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Create forms for each component
        var simulatorForm = new ComponentForm("SecureHome - Threat Simulator");
        var centerForm = new ComponentForm("SecureHome - Monitoring Center");
        var houseForm = new ComponentForm("SecureHome - House Security");

        // Position windows
        simulatorForm.StartPosition = FormStartPosition.Manual;
        simulatorForm.Location = new System.Drawing.Point(50, 50);

        centerForm.StartPosition = FormStartPosition.Manual;
        centerForm.Location = new System.Drawing.Point(700, 50);

        houseForm.StartPosition = FormStartPosition.Manual;
        houseForm.Location = new System.Drawing.Point(350, 400);

        // Create logger factories that write to forms
        var simLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new FormLoggerProvider(simulatorForm.WriteLog));
        });

        var centerLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new FormLoggerProvider(centerForm.WriteLog));
        });

        var houseLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new FormLoggerProvider(houseForm.WriteLog));
        });

        // Initialize components
        var simulator = new ThreatSimulatorConsole(simLoggerFactory, simulatorForm.WriteLog);
        var center = new MonitoringCenterConsole(centerLoggerFactory, centerForm.WriteLog);
        var house = new HouseSecurityConsole(houseLoggerFactory, houseForm.WriteLog);

        // Set up command handlers - route to actual component methods
        simulatorForm.OnCommand = cmd =>
        {
            simulator.HandleCommand(cmd);
        };

        centerForm.OnCommand = cmd =>
        {
            center.HandleOperatorCommand(cmd);
        };

        houseForm.OnCommand = cmd =>
        {
            house.HandleUserCommand(cmd);
        };

        // Show all forms first
        simulatorForm.Show();
        centerForm.Show();
        houseForm.Show();

        // Force handle creation and ensure forms are ready
        simulatorForm.CreateControl();
        centerForm.CreateControl();
        houseForm.CreateControl();

        // Wait a bit for UI to stabilize
        Application.DoEvents();
        Thread.Sleep(500);

        // Start components in background threads (same process!)
        // Each component will display its own startup messages
        Task.Run(() => simulator.Start());
        Task.Run(() => center.Start());
        Task.Run(() => house.Start());

        // Set initial status
        simulatorForm.SetStatus("AUTO MODE: 30s interval");
        centerForm.SetStatus("Monitoring - No alerts");
        houseForm.SetStatus("TODO 01 - Configure house");

        // Run application (keeps all windows open)
        Application.Run(simulatorForm);
    }
}

/// <summary>
/// Logger that writes to form
/// </summary>
public class FormLogger : ILogger
{
    private readonly Action<string> _writeAction;

    public FormLogger(Action<string> writeAction)
    {
        _writeAction = writeAction;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        _writeAction(message);
    }
}

public class FormLoggerProvider : ILoggerProvider
{
    private readonly Action<string> _writeAction;

    public FormLoggerProvider(Action<string> writeAction)
    {
        _writeAction = writeAction;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FormLogger(_writeAction);
    }

    public void Dispose() { }
}
