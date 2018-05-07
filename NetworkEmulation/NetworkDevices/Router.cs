using Communications;
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
        }

        private void StartRouting() {
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var msg = await queue.ReceiveAsync();
                    var target = msg.Split('/')[1];
                    if (routingTable.ContainsKey(target))
                    {
                        var i = routingTable[target];
						i.Send(msg);
                    }
                }
            });
        }

        private void StartInterfaces() =>
			Interfaces.ForEach(i => {
				Task.Factory.StartNew(async () => {
					while (true)
					{
						var recv = await i.Receive();
                        var msg = recv.Message;
						var source = msg.Split('/')[0];
						var target = msg.Split('/')[1];
                        var type = msg.Split('/')[2];
                        if (type == "SYN" && target == i.IpAddress)
						{
                            var added = false;
                            while (!added) {
                                added = routingTable.TryAdd(source, i);
                                Thread.Sleep(10);
                            }
                            i.Send($"{target}/{source}/ACK");
                        } else {
                            queue.Post(msg);
                        }
					}
				});
			});

    }
}
