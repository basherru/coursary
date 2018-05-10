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

            var R1 = new Router(new [] { "192.168.0.1", "192.168.0.2" });
            var R2 = new Router(new [] { "192.168.0.3", "192.168.0.4" });
            var R3 = new Router(new [] { "192.168.0.5", "192.168.0.6" });
            var R4 = new Router(new [] { "192.168.0.7", "192.168.0.8" });
            var R5 = new Router(new [] { "192.168.0.9", "192.168.0.10" });

            R1.Connect("192.168.0.2", "192.168.0.4");
            R2.Connect("192.168.0.3", "192.168.0.6");
            R3.Connect("192.168.0.5", "192.168.0.8");
            R4.Connect("192.168.0.7", "192.168.0.10");

            var C1 = new Computer("192.168.1.1", "192.168.0.1");
            var C2 = new Computer("192.168.2.1", "192.168.0.9");

            Thread.Sleep(2000);

            C1.Send("192.168.2.1/Hello, World!");
            C2.Send("192.168.1.1/Hello, There!");


            Thread.Sleep(2000);
            Console.ReadKey(true);

        }
    }
}
