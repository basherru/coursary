using System.Text;
using Helpers;

namespace Communications
{
    public class UdpClient: UdpBase
    {
        private UdpClient(int port) : base(port) { }

        public static UdpClient Create(int srcPort, int dstPort)
        {
            var connection = new UdpClient(srcPort);
            connection.Client.Connect("127.0.0.1", dstPort);
            return connection;
        }

		public static UdpClient Create(string ipAddress, int dstPort)
		{
            var connection = new UdpClient(ipAddress.HashCode());
			connection.Client.Connect("127.0.0.1", dstPort);
			return connection;
		}

        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }
    }
}
