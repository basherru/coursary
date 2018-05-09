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

            var router1 = new Router(new [] { "192.168.0.1", "192.168.0.2", "192.168.0.3" });
            var router2 = new Router(new [] { "192.168.0.4", "192.168.0.5", "192.168.0.6" });

            var computer1 = new Computer("192.168.0.7", "192.168.0.1");
            var computer2 = new Computer("192.168.0.8", "192.168.0.2");

			var computer3 = new Computer("192.168.0.9", "192.168.0.4");
			var computer4 = new Computer("192.168.0.10", "192.168.0.5");

            router1.Connect("192.168.0.3", "192.168.0.6");
            
            Thread.Sleep(1000);
            
            computer1.Send("192.168.0.8/1-2");
            computer2.Send("192.168.0.9/2-3");
			computer3.Send("192.168.0.7/3-1");
			computer4.Send("192.168.0.9/4-3");

            Thread.Sleep(2000);
            Console.ReadKey(true);

        }
    }
}
