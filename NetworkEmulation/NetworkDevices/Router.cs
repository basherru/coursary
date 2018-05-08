using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using System.Linq;
using System;

namespace NetworkDevices
{
    public sealed class Router
    {

        public readonly List<NetworkInterface> Interfaces;
        private readonly ConcurrentDictionary<string, NetworkInterface> routingTable = new ConcurrentDictionary<string, NetworkInterface>();
        private readonly BufferBlock<string> queue = new BufferBlock<string>();

        public Router(string[] ipAddresses)
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
                    var target = msg.Split('/')[1];
                    if (routingTable.ContainsKey(target))
                    {
                        var i = routingTable[target];
                        i.Send(msg);
                        Console.WriteLine($"Routing {msg} to {target} through {i.IpAddress}");
                    }
                }
            });

        public void Connect(string sourceIp, string destinationIp) {
            var srcInterface = Interfaces.Where(i => i.IpAddress == sourceIp).First();
            srcInterface.Send(destinationIp, $"{sourceIp}/{destinationIp}/SYN/{NetworkDeviceType.ROUTER}");
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
                        if ((message == "SYN" || message == "ACK") && target == i.IpAddress)
                        {
                            if (message == "SYN")
                            {
                                Console.WriteLine($"Interface {i.IpAddress}: Recieved SYN from {source}");
                                i.Send($"{target}/{source}/ACK/{NetworkDeviceType.ROUTER}");
                            }
                            else
                            {
                                Console.WriteLine($"Interface {i.IpAddress}: Recieved ACK from {source}");
                            }
                            i.Type = type;
                            AddToRoutingTable(source, i);
                        }
                        else if (message == "DIST")
                        {
                            var recievedIps = msg.Split('/')[4];
                            var distibutedIps = Deserialize(recievedIps);
                            distibutedIps.Where(ip => ip != i.IpAddress).ToList().ForEach(ip => AddToRoutingTable(ip, i));
                        }
                        else
                        {
                            queue.Post(msg);
                        }
                    }
                });
            });

        private void AddToRoutingTable(string ip, NetworkInterface i) {
			var added = false;
			while (!added)
			{
				added = routingTable.TryAdd(ip, i);
				Thread.Sleep(10);
			}
		}

        private void StartTableDistribution() =>
            Task.Factory.StartNew(() => {
                while (true) {
                    Interfaces
                    .Where(i => i.Type == NetworkDeviceType.ROUTER)
                    .ToList()
                    .ForEach(i => {
    					var serializedTable = Serialize(routingTable);
    					i.Send($"{i.IpAddress}/192.168.0.255/DIST/{NetworkDeviceType.ROUTER}/{serializedTable}");
                    });
                    Thread.Sleep(100);
                }
            });

        private static string Serialize(ConcurrentDictionary<string, NetworkInterface> table) {
            string data = string.Empty;
            foreach (var key in table.Keys) {
                data += $"{key}#";
            }
            if (data.EndsWith("#", StringComparison.OrdinalIgnoreCase)) {
                data = data.Substring(0, data.Length - 1);
            }
            return data;
        }

		private static string[] Deserialize(string data)
		{
            if (data.Length == 0) {
                return new string[0];
            }
            var result = data.Split('#');
            return result;
        }

    }
}
