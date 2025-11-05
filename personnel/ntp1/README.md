# NTP

## A. Récupération de l'heure actuelle à partir d'un serveur NTP

Objectif : Écrire un programme qui récupère l'heure actuelle à partir d'un serveur NTP et l'affiche à l'écran.

Instructions :

1. Créez un nouveau projet console en C# dans Visual Studio.
2. Ajoutez la bibliothèque `System.Net.Sockets` à votre projet.
3. Dans le fichier `Program.cs`, importez les espaces de noms suivants :

```csharp
using System;
using System.Net;
using System.Net.Sockets;
```

4. Dans la méthode `Main()`, créez une variable `ntpServer` qui contient l'adresse IP ou le nom de domaine d'un serveur
   NTP public, tel que `0.ch.pool.ntp.org`.

```csharp
string ntpServer = "0.ch.pool.ntp.org";
```

5. Créez une variable `timeMessage` de type `byte[]` avec une taille de 48 octets et l'initialiser ainsi :

```csharp
byte[] timeMessage = new byte[48];
ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
```

6. Créez une variable `ntpReference` de type `IPEndPoint` en utilisant le port 123 (port
   standard du protocole NTP).

```csharp
IPEndPoint ntpReference = new IPEndPoint(Dns.GetHostAddresses(ntpServer)[0], 123);
```

7. Créez une variable `client` de type `UdpClient` et connectez-la au serveur NTP.

```csharp
UdpClient client = new UdpClient();
ntpClient.Connect(client);
```

8. Envoyez une demande NTP au serveur en utilisant la méthode `Send()` de la classe `UdpClient`.

```csharp
client.Send(timeMessage, timeMessage.Length);
```

9. Recevez la réponse NTP du serveur en utilisant la méthode `Receive()` de la classe `UdpClient`.

```csharp
timeMessage = client.Receive(ref ntpReference);
```

10. Convertissez les données NTP reçues en un objet `DateTime` en utilisant la méthode `ToDateTime()` de la
    classe `NtpPacket`.

```csharp
DateTime ntpTime = NtpPacket.ToDateTime(ntpData);
```

<details><summary>NtpPacket ??</summary>

Voici de quoi remplir NtpPacket
```csharp
ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
```

</details>

11. Affichez l'heure actuelle à l'écran en utilisant la méthode `ToString()` de la classe `DateTime`.

```csharp
Console.WriteLine($"Heure actuelle : {ntpTime}");
```

12. Fermez la connexion au serveur NTP en utilisant la méthode `Close()` de la classe `UdpClient`.

```csharp
client.Close();
```

13. Exécutez le programme et vérifiez que l'heure actuelle est affichée correctement à l'écran.

## B. Formats de date

### 1. Afficher l'heure actuelle dans différents formats de date :
    
    - lundi, 16 août 2021
    - 25.10.2024 08:00:00
    - 10.10.2024

<details><summary>Aide</summary>

```csharp
Console.WriteLine($"Heure actuelle (format personnalisé) : {ntpTime.ToString("dd/MM/yyyy HH:mm:ss")}");
```
</details>

### 2. ISO 8601
	Afficher maintenant l’heure au format iso 8601
	- 2025-11-12T21:21:21Z

## C. Conversions temporelles

### 1. Calculer la différence de temps entre l'heure locale et l'heure NTP :

<details><summary>Voir la réponse</summary>

```csharp
// 1. Calculate time difference (both in UTC)
DateTime ntpTimeUtc = ntpTime; // Already UTC from NTP
DateTime systemTimeUtc = DateTime.UtcNow;
TimeSpan timeDiff = systemTimeUtc - ntpTimeUtc;
Console.WriteLine($"Différence de temps : {timeDiff.TotalSeconds:F2} secondes");
```
</details>


### 2. Corriger l'heure actuelle avec l'heure NTP et afficher le tout en heure locale :

<details><summary>Voir la réponse</summary>

```csharp
// 2. Convert to local time zone properly
DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ntpTimeUtc, TimeZoneInfo.Local);
Console.WriteLine($"Heure locale : {localTime}");
```
</details>


### 3. Convertir l'heure locale en heure UTC :

<details><summary>Voir la réponse</summary>

```csharp
// 3. Convert to specific time zones
TimeZoneInfo swissTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
DateTime swissTime = TimeZoneInfo.ConvertTimeFromUtc(ntpTimeUtc, swissTimeZone);
Console.WriteLine($"Heure suisse : {swissTime}");
```
</details>


### 4. Convertir l'heure UTC en heure locale :


<details><summary>Voir la réponse</summary>

```csharp
TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
DateTime backToUtc = TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, utcTimeZone);
Console.WriteLine($"Retour vers UTC : {backToUtc}");
```
</details>


## D. Amélioration du programme de base
Une connexion réseau implique des problématiques dont la principale est la qualité de la connexion...
Il est donc pertinent de prendre en considération le ‘**délai de réponse**‘.
De plus, un client réseau en C# consomme des ressources et pour éviter d’oublier la fermeture de la connexion,
il est judicieux d’utiliser la ‘**gestion automatique des ressources**‘.

### Gestion des ressources
Encapsuler le code dans un bloc ‘using’:
``` csharp
using (UdpClient client = ...)
{
    //Insert code here
}
```

## E. Pool de serveurs
Se baser sur NTP implique une faille potentielle de disponibilité. Pour la relativiser, il est commun d’utiliser
un pool de serveurs:

``` csharp
// NTP Server Pool and Reliability
string[] ntpServers = {
    "0.pool.ntp.org",
    "1.pool.ntp.org", 
    "time.google.com",
    "time.cloudflare.com"
};

// TODO : Try multiple servers for reliability
```

## F. Horloge mondiale
Afficher l’heure des villes suivantes (en plus d’UTC):
 - New York 
 - Londres
 - Tokyo
 - Sydney
 
<detail>
<summary>
Voir la réponse
</summary>

``` csharp
// Exercise F: World Clock Display
public static void DisplayWorldClocks(DateTime utcTime)
{
    var timeZones = new[]
    {
        ("UTC", TimeZoneInfo.Utc),
        ("New York", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
        ("London", TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")),
        ("Tokyo", TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time")),
        ("Sydney", TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"))
    };
    
    foreach (var (name, tz) in timeZones)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
        Console.WriteLine($"{name}: {localTime:yyyy-MM-dd HH:mm:ss}");
    }
}
```

</detail>

G. NodaTime et drift
## G. NodaTime Integration et Manipulation du Drift

### Installation et Configuration

#### 1. Installation des packages NuGet
```csharp
// Dans le Package Manager Console :
// Install-Package NodaTime
// Install-Package NodaTime.NetworkClock
```

#### 2. Configuration de base avec NodaTime
Remplacez votre code NTP existant par cette implémentation avec NodaTime :

```csharp
using System;
using NodaTime;
using NodaTime.NetworkClock;
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
    Offset drift = systemTime - ntpTime;
    Console.WriteLine($"Drift détecté: {drift.Nanoseconds / 1_000_000.0:F3} ms");
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur NetworkClock: {ex.Message}");
    // Fallback sur l'horloge système
}
```

### 3. Analyse Avancée du Drift

#### A. Mesure Précise du Drift

Le drift représente l'écart entre l'horloge locale et le temps de référence, qui évolue au fil du temps. Voici une classe pour analyser le drift :

```csharp
public class ClockDriftAnalyzer
{
    private readonly List<DriftMeasurement> _measurements = new();
    private readonly NetworkClock _networkClock;
    private readonly IClock _systemClock;
    
    public ClockDriftAnalyzer()
    {
        _networkClock = NetworkClock.Instance;
        _systemClock = SystemClock.Instance;
    }
    
    public class DriftMeasurement
    {
        public Instant Timestamp { get; set; }
        public Offset SystemOffset { get; set; }
        public Duration NetworkLatency { get; set; }
        public double DriftRatePpm { get; set; } // Parts per million
    }
    
    public async Task<DriftMeasurement> MeasureDriftAsync()
    {
        var startTime = _systemClock.GetCurrentInstant();
        
        try
        {
            // Mesurer le temps réseau avec plusieurs tentatives
            var measurements = new List<(Instant ntpTime, Duration latency)>();
            
            for (int i = 0; i < 5; i++)
            {
                var before = _systemClock.GetCurrentInstant();
                var ntpTime = _networkClock.GetCurrentInstant();
                var after = _systemClock.GetCurrentInstant();
                
                var latency = after - before;
                measurements.Add((ntpTime, latency));
                
                await Task.Delay(100);
            }
            
            // Sélectionner la mesure avec la latence minimale
            var bestMeasurement = measurements.OrderBy(m => m.latency).First();
            var systemTime = _systemClock.GetCurrentInstant();
            
            var drift = new DriftMeasurement
            {
                Timestamp = systemTime,
                SystemOffset = systemTime - bestMeasurement.ntpTime,
                NetworkLatency = bestMeasurement.latency,
                DriftRatePpm = CalculateDriftRate()
            };
            
            _measurements.Add(drift);
            return drift;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur mesure drift: {ex.Message}");
            return null;
        }
    }
    
    private double CalculateDriftRate()
    {
        if (_measurements.Count < 2) return 0.0;
        
        var first = _measurements.First();
        var last = _measurements.Last();
        
        var timeDiff = last.Timestamp - first.Timestamp;
        var offsetDiff = last.SystemOffset.Nanoseconds - first.SystemOffset.Nanoseconds;
        
        if (timeDiff.Nanoseconds == 0) return 0.0;
        
        // Calcul en parts par million (ppm)
        return (double)offsetDiff / timeDiff.Nanoseconds * 1_000_000;
    }
    
    public void DisplayDriftStatistics()
    {
        if (_measurements.Count == 0)
        {
            Console.WriteLine("Aucune mesure disponible");
            return;
        }
        
        var avgOffset = _measurements.Average(m => m.SystemOffset.Nanoseconds) / 1_000_000.0;
        var maxOffset = _measurements.Max(m => Math.Abs(m.SystemOffset.Nanoseconds)) / 1_000_000.0;
        var avgLatency = _measurements.Average(m => m.NetworkLatency.Nanoseconds) / 1_000_000.0;
        var currentDriftRate = _measurements.Last().DriftRatePpm;
        
        Console.WriteLine("=== STATISTIQUES DE DRIFT ===");
        Console.WriteLine($"Nombre de mesures: {_measurements.Count}");
        Console.WriteLine($"Offset moyen: {avgOffset:F3} ms");
        Console.WriteLine($"Offset maximal: {maxOffset:F3} ms");
        Console.WriteLine($"Latence réseau moyenne: {avgLatency:F3} ms");
        Console.WriteLine($"Taux de drift actuel: {currentDriftRate:F2} ppm");
        
        // Classification de la qualité
        if (Math.Abs(avgOffset) < 1.0)
            Console.WriteLine("Qualité: EXCELLENTE (< 1ms)");
        else if (Math.Abs(avgOffset) < 10.0)
            Console.WriteLine("Qualité: BONNE (< 10ms)");
        else if (Math.Abs(avgOffset) < 100.0)
            Console.WriteLine("Qualité: ACCEPTABLE (< 100ms)");
        else
            Console.WriteLine("Qualité: PROBLÉMATIQUE (> 100ms)");
    }
}
```

Et voici une classe pas si intelligente mais qui a au moins un peu de mémoire:

```csharp
public class PredictiveDriftCorrector
{
    private readonly ClockDriftAnalyzer _analyzer;
    private Offset _predictedDrift = Offset.Zero;
    
    public PredictiveDriftCorrector(ClockDriftAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }
    
    public Instant GetCorrectedTime()
    {
        var systemTime = SystemClock.Instance.GetCurrentInstant();
        return systemTime - _predictedDrift;
    }
    
    public async Task CalibrateAsync(int measurementCount = 10, Duration interval = default)
    {
        if (interval == default) interval = Duration.FromSeconds(30);
        
        Console.WriteLine($"Calibration en cours ({measurementCount} mesures)...");
        
        for (int i = 0; i < measurementCount; i++)
        {
            var measurement = await _analyzer.MeasureDriftAsync();
            if (measurement != null)
            {
                _predictedDrift = measurement.SystemOffset;
                Console.WriteLine($"Mesure {i + 1}/{measurementCount}: " +
                                $"Offset = {measurement.SystemOffset.Nanoseconds / 1_000_000.0:F3} ms, " +
                                $"Latence = {measurement.NetworkLatency.Nanoseconds / 1_000_000.0:F3} ms");
            }
            
            if (i < measurementCount - 1)
                await Task.Delay((int)interval.TotalMilliseconds);
        }
        
        Console.WriteLine("Calibration terminée.");
        _analyzer.DisplayDriftStatistics();
    }
}
```

##### Logique du Prédicteur de Drift

Le `PredictiveDriftCorrector` fonctionne selon une logique simple mais efficace :

###### Principe de Base
1. **Mesure du décalage** : Il mesure régulièrement l'écart (offset) entre l'horloge système locale et le temps NTP de référence
2. **Mémorisation** : Il stocke la dernière mesure de drift dans `_predictedDrift`
3. **Correction prédictive** : Quand on demande l'heure "corrigée", il soustrait ce drift de l'heure système actuelle

##### Exemple Concret
```csharp
// Si l'horloge système avance de 50ms par rapport à NTP
_predictedDrift = +50ms

// Pour obtenir l'heure corrigée :
heureCorrigee = heureSysteme - 50ms  // On "recule" de 50ms
```

##### Limitations
- **Prédiction simpliste** : Il assume que le drift reste constant entre les mesures
- **Pas d'extrapolation** : Il ne prédit pas l'évolution future du drift basée sur les tendances passées
- **Correction statique** : Une vraie prédiction analyserait le taux de dérive (ppm) pour projeter le drift futur

C'est donc plus un **correcteur basé sur la dernière mesure** qu'un véritable prédicteur, mais suffisant pour des applications nécessitant une précision modérée.

### 4. Exercices Pratiques avec NodaTime
	
### Exercice A : Moniteur de Drift en Temps Réel
Compléter ce programme qui surveille le drift toutes les minutes :

```csharp
public static async Task RunDriftMonitorAsync()
{
    var analyzer = new ClockDriftAnalyzer();
    var corrector = new PredictiveDriftCorrector(analyzer);
    
    // Calibration initiale
    await corrector.CalibrateAsync(5, Duration.FromSeconds(10));
    
    // Surveillance continue
    var timer = new Timer(async _ =>
    {
        var measurement = await analyzer.MeasureDriftAsync();
        if (measurement != null)
        {
			//TODO : Afficher le drift et le temps corrigé
        }
    }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    
    Console.WriteLine("Appuyez sur une touche pour arrêter la surveillance...");
    Console.ReadKey();
    timer.Dispose();
}
```

### Exercice B : Comparaison Multi-Serveurs
NetworkClock peut être configuré avec différents serveurs NTP, compléter le programme suivant :

```csharp
public class MultiServerDriftAnalysis
{
    private readonly string[] _ntpServers = {
        "0.pool.ntp.org",
        "1.pool.ntp.org", 
        "time.google.com",
        "time.cloudflare.com"
    };
    
    public async Task CompareServersAsync()
    {
        var systemTime = SystemClock.Instance.GetCurrentInstant();
        
        Console.WriteLine("=== COMPARAISON MULTI-SERVEURS ===");
        Console.WriteLine($"Heure système: {systemTime}");
        Console.WriteLine();
        
        foreach (var server in _ntpServers)
        {
            try
            {
                var clock = new NetworkClock
                {
                    NtpServer = server,
                    CacheTimeout = Duration.FromSeconds(30)
                };
                
                var startTime = SystemClock.Instance.GetCurrentInstant();
                var ntpTime = clock.GetCurrentInstant();
                var endTime = SystemClock.Instance.GetCurrentInstant();
                
                var latency = 0 /*TODO calculer la latence*/;
                var offset = 0 /*TODO calculer l'offset*/;
                
                Console.WriteLine($"Serveur: {server}");
                Console.WriteLine($"  Offset: {offset.Nanoseconds / 1_000_000.0:F3} ms");
                Console.WriteLine($"  Latence: {latency.Nanoseconds / 1_000_000.0:F3} ms");
                Console.WriteLine($"  Heure NTP: {ntpTime}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Serveur {server}: ERREUR - {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
```

### Exercice C : Horloge Mondiale avec Correction de Drift
Compléter ce programme qui combiner la correction de drift avec l'affichage multi-zones :

```csharp
public static async Task DisplayCorrectedWorldClocksAsync()
{
    var analyzer = new ClockDriftAnalyzer();
    var corrector = new PredictiveDriftCorrector(analyzer);
    
    // Calibration rapide
    await corrector.CalibrateAsync(3, Duration.FromSeconds(5));
    
    var timeZones = new[]
    {
        ("UTC", DateTimeZoneProviders.Tzdb["UTC"]),
        ("New York", DateTimeZoneProviders.Tzdb["America/New_York"]),
        ("London", DateTimeZoneProviders.Tzdb["Europe/London"]),
        ("Tokyo", DateTimeZoneProviders.Tzdb["Asia/Tokyo"]),
        ("Sydney", DateTimeZoneProviders.Tzdb["Australia/Sydney"])
    };
    
    var correctedUtc = corrector.GetCorrectedTime();
    
    Console.WriteLine("=== HORLOGE MONDIALE (CORRIGÉE) ===");
    Console.WriteLine($"Temps corrigé UTC: {correctedUtc}");
    Console.WriteLine();
    
    foreach (var (name, tz) in timeZones)
    {
        var localTime = correctedUtc.InZone(tz);
        //TODO Afficher le nom, la date actuelle et la correction
    }
}
```
	
### 5. Missions supplémentaires

#### Détecteur avec seuil
Implémenter une fonction qui détecte si le drift dépasse un seuil critique (ex: 100ms) et recommande une resynchronisation forcée.

#### Performance
NTP peut maintenir une précision de quelques dizaines de millisecondes sur Internet public. Créez un test de performance qui mesure la précision atteinte avec votre implémentation.

#### Sérialisation preview
Ajouter une fonctionnalité de persistence qui sauvegarde les mesures de drift dans un fichier JSON et les restore au démarrage du programme.

