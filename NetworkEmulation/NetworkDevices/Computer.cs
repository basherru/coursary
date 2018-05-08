using System;
using System.Threading.Tasks;
using Communications;
using Helpers;

namespace NetworkDevices
{
    public class Computer
    {

        public string IpAddress;
        public UdpClient client;
        protected string interfaceIp;

        public Computer(string ipAddress, string interfaceIp)
        {
            IpAddress = ipAddress;
            this.interfaceIp = interfaceIp;
            client = new UdpClient(ipAddress);
            client.Send(interfaceIp, $"{ipAddress}/{interfaceIp}/SYN/{NetworkDeviceType.COMPUTER}");
            StartLogging();
        }

        public void Send(string message) => client.Send(interfaceIp, $"{IpAddress}/{message}/{NetworkDeviceType.COMPUTER}");

        public async Task<Received> Receive() => await client.Receive();

        public void StartLogging()
        {
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var msg = await Receive();
                    Console.WriteLine($"Computer {IpAddress}: Recieved {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
                }
            });
        }

    }
}
