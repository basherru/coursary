using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Communications;
using Helpers;

namespace NetworkDevices
{
    public class NetworkInterface : UdpClient
    {

        public string IpAddress { get; private set; }
        public string Type { get; set; }

        public NetworkInterface(string IpAddress) : base(IpAddress)
        {
            this.IpAddress = IpAddress;
            Type = NetworkDeviceType.NONE;
        }

        private new void SendBroadcast(string message) {}

        public void Send(string message) {
            base.SendBroadcast(message);
        }

        public static NetworkInterface New(string IpAddress) => new NetworkInterface(IpAddress);

    }
}
