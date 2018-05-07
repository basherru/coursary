using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Communications;
using NetworkPackets;
using Helpers;
using NetworkDevices;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {

            var router = new Router(new [] { "192.168.0.1", "192.168.0.2", "192.168.0.3", "192.168.0.4" });

            var computer1 = new Computer("192.168.0.5", "192.168.0.1");
            var computer2 = new HackerComputer("192.168.0.6", "192.168.0.2");

            computer1.Send("192.168.0.6/Hello, World");
            computer2.Send("192.168.0.5/192.168.0.5/Hello, there!");

            Thread.Sleep(2000);

        }
    }
}
