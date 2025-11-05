using System;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace ntp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ntpServer = "0.ch.pool.ntp.org";
            byte[] timeMessage = new byte[48];
            timeMessage[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mod
            IPEndPoint ntpReference = new IPEndPoint(Dns.GetHostAddresses(ntpServer)[0], 123);

            using (UdpClient client = new UdpClient())
            {
                client.Connect(ntpReference);
                client.Send(timeMessage, timeMessage.Length);
                timeMessage = client.Receive(ref ntpReference);

                ulong intPart = (ulong)timeMessage[40] << 24 | (ulong)timeMessage[41] << 16 | (ulong)timeMessage[42] << 8 | (ulong)timeMessage[43];
                ulong fractPart = (ulong)timeMessage[44] << 24 | (ulong)timeMessage[45] << 16 | (ulong)timeMessage[46] << 8 | (ulong)timeMessage[47];

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                Console.WriteLine(networkDateTime.ToLongDateString());
                Console.WriteLine(networkDateTime);
                Console.WriteLine(networkDateTime.ToShortDateString());
                Console.WriteLine($"Heure actuelle (format personnalisé) : {networkDateTime.ToString("dd/MM/yyyy HH:mm:ss")}");
                Console.WriteLine(networkDateTime.ToString("u", CultureInfo.InvariantCulture));
                Console.WriteLine(networkDateTime.ToString("s", CultureInfo.InvariantCulture));
                Console.WriteLine(networkDateTime.ToString("o", CultureInfo.InvariantCulture));

                Console.WriteLine(networkDateTime.Subtract(DateTime.Now));

                // 2. Convert to local time zone properly
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(networkDateTime.ToUniversalTime(), TimeZoneInfo.Local);
                Console.WriteLine($"Heure locale : {localTime}");

                Console.WriteLine(localTime.ToUniversalTime());
                TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
                DateTime backToUtc = TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, utcTimeZone);
                Console.WriteLine($"Retour vers UTC : {backToUtc}");

                // 3. Convert to specific time zones
                TimeZoneInfo swissTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                DateTime swissTime = TimeZoneInfo.ConvertTimeFromUtc(networkDateTime, swissTimeZone);
                Console.WriteLine($"Heure suisse : {swissTime}");
            }
        }
    }
}
