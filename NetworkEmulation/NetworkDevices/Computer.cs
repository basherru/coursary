using System;
using System.Threading.Tasks;
using Communications;
using Helpers;
using NetworkPackets;

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
            client.Send(interfaceIp, $"{ipAddress}/{interfaceIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.COMPUTER}");
            StartLogging();
        }

        private bool IsEcho(string msg) => msg.Contains(PacketType.ICMP.ICMP_ECHO_REQUEST);
        private bool IsBroadcast(string msg) => string.Compare(msg.Split('/')[1], "255.255.255.255") == 0;

        public void Send(string destinationIp, string message) => client.Send(interfaceIp, $"{IpAddress}/{destinationIp}/{message}/{NetworkDeviceType.COMPUTER}");

        public async Task<Received> Receive() {
			var recv = await client.Receive();
			var msg = recv.Message;
			if (IsEcho(msg))
			{
				var source = msg.Split('/')[0];
				Send(source, PacketType.ICMP.ICMP_ECHO_REPLY);
			}
			return recv;
        }

        public void StartLogging()
        {
            Task.Factory.StartNew(async () => {
                while (true)
                {                    
                    var msg = await Receive();
                    var target = msg.Message.Split('/')[1];
                    if (target == IpAddress || IsBroadcast(msg.Message)) {
                        Console.WriteLine($"Computer {IpAddress}: Recieved {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");    
                    } else {
                        Console.WriteLine($"Computer {IpAddress}: Discarded {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
                    }

                }
            });
        }

    }
}
