using System;
using System.Threading.Tasks;
using Communications;
using Helpers;

namespace NetworkDevices
{
	public class HackerComputer
	{

		public string IpAddress;
		public UdpClient client;

		public HackerComputer(string ipAddress, string connectedRouterIp)
		{
			IpAddress = ipAddress;
			client = UdpClient.Create(ipAddress.HashCode(), connectedRouterIp.HashCode());
			client.Send($"{ipAddress}/{connectedRouterIp}/SYN");
			StartLogging();
		}

		public void Send(string message) => client.Send(message);

		public async Task<Received> Receive() => await client.Receive();

		public void StartLogging()
		{
			Task.Factory.StartNew(async () => {
				while (true)
				{
					var msg = await Receive();
					Console.WriteLine($"Hacker Computer {IpAddress}: Recieved {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
				}
			});
		}

	}
}
