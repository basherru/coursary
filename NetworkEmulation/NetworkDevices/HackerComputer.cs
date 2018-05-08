using System;
using System.Threading.Tasks;
using Communications;
using Helpers;

namespace NetworkDevices
{
    public class HackerComputer : Computer
	{

        public HackerComputer(string ipAddress, string interfaceIp) : base(ipAddress, interfaceIp) {}

		public new void Send(string message) => client.Send(interfaceIp, $"message/{NetworkDeviceType.COMPUTER}");

	}
}
