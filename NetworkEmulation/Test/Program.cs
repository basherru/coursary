using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Communications;
using NetworkPackets;
using Helpers;
using NetworkDevices;
using System.Net;
using System.Text;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {

            var R1 = new Router(new [] { "192.168.0.1", "192.168.0.2", "192.168.0.3", "192.168.0.4", "192.168.0.5", "192.168.0.6" });

            var C1 = new Computer("192.168.1.0", "192.168.0.1");
            var C2 = new Computer("192.168.2.0", "192.168.0.2");
            var C3 = new Computer("192.168.3.0", "192.168.0.3");
            var C4 = new Computer("192.168.4.0", "192.168.0.4");
            var C5 = new Computer("192.168.5.0", "192.168.0.5");

            var H1 = new HackerComputer("192.168.6.0", "192.168.0.6");

            Thread.Sleep(1000);

            H1.Send("192.168.1.0", "255.255.255.255", PacketType.ICMP.ICMP_ECHO_REQUEST);

            Thread.Sleep(500);
            Console.ReadKey(true);

        }
    }
}
