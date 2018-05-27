using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using System.Linq;
using System;
using NetworkPackets;

namespace NetworkDevices
{
	public sealed class Switch
	{

		public readonly List<NetworkInterface> Interfaces;
		private readonly ConcurrentDictionary<string, ISet<NetworkInterface>> routingTable = new ConcurrentDictionary<string, ISet<NetworkInterface>>();
		private readonly BufferBlock<string> queue = new BufferBlock<string>();

		public Switch(string[] ipAddresses)
		{
			Interfaces = ipAddresses.Select(NetworkInterface.New).ToList();
			StartInterfaces();
			StartRouting();
            StartTableDistribution();
		}

		private void StartRouting() =>
			Task.Factory.StartNew(async () => {
				while (true)
				{
					var msg = await queue.ReceiveAsync();
                    var source = msg.Split('/')[0];
					var target = msg.Split('/')[1];
                    var interfaceIp = msg.Split('/').Last();
                    msg = msg.Replace($"/{interfaceIp}", "");
                    if (routingTable.ContainsKey(target))
					{
						var interfaces = routingTable[target];
						var i = interfaces.OrderBy(x => x.Type == NetworkDeviceType.ROUTER).First();
						i.Send(msg);
						Console.WriteLine($"Routing {msg} to {target} through {i.IpAddress}");
                    } else {
                        Interfaces.Where(i => i.IpAddress != interfaceIp).ToList().ForEach(i => i.Send(msg));
                        AddToRoutingTable(source, Interfaces.First(i => i.IpAddress == interfaceIp));
                        Console.WriteLine($"Broadcasting {msg} from {interfaceIp}");
                    }
				}
			});

		public void Connect(string sourceIp, string destinationIp)
		{
			var srcInterface = Interfaces.First(i => i.IpAddress == sourceIp);
            srcInterface.Send(destinationIp, $"{sourceIp}/{destinationIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.SWITCH}");
		}

		private void StartInterfaces() =>
			Interfaces.ForEach(i =>
			{
				Task.Factory.StartNew(async () =>
				{
					while (true)
					{
						var data = await i.Receive();
						var msg = data.Message;
						var source = msg.Split('/')[0];
						var target = msg.Split('/')[1];
						var message = msg.Split('/')[2];
						var type = msg.Split('/')[3];
						Console.WriteLine($"SWITCH GOT: {msg}");
						if ((message == PacketType.HANDSHAKE.SYN || message == PacketType.HANDSHAKE.ACK) && target == i.IpAddress)
						{
							if (message == PacketType.HANDSHAKE.SYN)
							{
								Console.WriteLine($"Interface {i.IpAddress}: Recieved SYN from {source}");
								i.Send($"{target}/{source}/ACK/{NetworkDeviceType.SWITCH}");
							}
							else
							{
								Console.WriteLine($"Interface {i.IpAddress}: Recieved ACK from {source}");
							}
							i.Type = type;
							AddToRoutingTable(source, i);
						} else
						{
                            queue.Post($"{msg}/{i.IpAddress}");
						}
					}
				});
			});

		private void AddToRoutingTable(string ip, NetworkInterface i)
		{
			if (routingTable.ContainsKey(ip))
			{
				var interfaces = routingTable[ip];
				interfaces.Add(i);
			}
			else
			{
				var interfaces = new HashSet<NetworkInterface> { i };
				routingTable[ip] = interfaces;
			}
		}

		private void StartTableDistribution() =>
			Task.Factory.StartNew(() => {
				while (true)
				{
					Interfaces
					.Where(i => i.Type == NetworkDeviceType.ROUTER)
					.ToList()
					.ForEach(i => {
						var serializedTable = Serialize(routingTable);
						var interfaces = string.Join("#", Interfaces.Select(x => x.IpAddress));
						var routingInfo = Merge(serializedTable, interfaces);
						i.Send($"{i.IpAddress}/192.168.0.255/DIST/{NetworkDeviceType.ROUTER}/{routingInfo}");
					});
					Thread.Sleep(100);
				}
			});

		private static string Serialize(IDictionary<string, ISet<NetworkInterface>> table) => string.Join("#", table.Keys);

		private static string Merge(string s1, string s2)
		{
			if (s1.Length > 0 && s2.Length > 0)
			{
				return string.Join("#", s1, s2);
			}
			if (s1.Length > 0)
			{
				return s1;
			}
			return s2;
		}

	}
}
