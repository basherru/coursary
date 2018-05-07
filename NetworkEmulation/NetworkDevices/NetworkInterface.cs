using System.Net;
using System.Text;
using System.Threading.Tasks;
using Communications;
using Helpers;

namespace NetworkDevices
{
    public class NetworkInterface : UdpServer
    {

        public string IpAddress;

        public NetworkInterface(string IpAddress) : base(IpAddress.HashCode())
        {
            this.IpAddress = IpAddress;
        }

        private new void SendBroadcast(string message) {}

        private new void Send(string message, IPEndPoint endpoint) {}

        public void Send(string message) {
            base.SendBroadcast(message);
        }

        public static NetworkInterface New(string IpAddress) => new NetworkInterface(IpAddress);

    }
}
