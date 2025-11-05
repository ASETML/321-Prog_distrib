using System;
using NodaTime;
using static NodaTime.NetworkClock;
using System.Threading.Tasks;
using NodaTime.TimeZones;

// Configuration du NetworkClock
var networkClock = NetworkClock.Instance;
networkClock.NtpServer = "pool.ntp.org";
networkClock.CacheTimeout = Duration.FromMinutes(15);

try
{
    // Obtenir l'heure précise via NTP
    Instant ntpTime = networkClock.GetCurrentInstant();
    Instant systemTime = SystemClock.Instance.GetCurrentInstant();

    Console.WriteLine($"Heure NTP (UTC): {ntpTime}");
    Console.WriteLine($"Heure système (UTC): {systemTime}");

    // Calcul du drift initial
    Offset drift = Offset.FromMilliseconds(int.Parse(((systemTime - ntpTime).TotalMilliseconds).ToString()));
    Console.WriteLine($"Drift détecté: {drift.Nanoseconds / 1_000_000.0:F3} ms");
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur NetworkClock: {ex.Message}");
    // Fallback sur l'horloge système
}