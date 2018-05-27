using System;
using System.Threading.Tasks;
using Communications;
using Helpers;
using NetworkPackets;

namespace NetworkDevices
{
	public class HackerComputer
	{

		public string IpAddress;
		public UdpClient client;
		protected string interfaceIp;

		public HackerComputer(string ipAddress, string interfaceIp)
		{
			IpAddress = ipAddress;
			this.interfaceIp = interfaceIp;
			client = new UdpClient(ipAddress);
            client.Send(interfaceIp, $"{ipAddress}/{interfaceIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.COMPUTER}");
			StartLogging();
		}

        public void Send(string sourceIp, string destinationIp, string message) => client.Send(interfaceIp, $"{sourceIp}/{destinationIp}/{message}/{NetworkDeviceType.COMPUTER}");

		public async Task<Received> Receive() => await client.Receive();

		public void StartLogging()
		{
			Task.Factory.StartNew(async () => {
				while (true)
				{
					var msg = await Receive();
					var target = msg.Message.Split('/')[1];
					if (target == IpAddress)
					{
						Console.WriteLine($"Computer {IpAddress}: Recieved {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
					}
					else
					{
						Console.WriteLine($"Computer {IpAddress}: Discarded {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
					}

				}
			});
		}

	}
}