using System.Text;
using Helpers;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communications
{
    public class UdpClient
    {

        private readonly System.Net.Sockets.UdpClient client;
        protected readonly ISet<IPEndPoint> _connections = new HashSet<IPEndPoint>();

        public int Port { get; set; }

        public UdpClient(string ipAddress) {
            Port = ipAddress.HashCode();
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            client = new System.Net.Sockets.UdpClient(endpoint);
        }

        public void Send(string ipAddress, string message) {
            var port = ipAddress.HashCode();
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            var data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, endpoint);
        }

		public void SendBroadcast(string message)
		{
			foreach (var endpoint in _connections)
			{
				try
				{
                    var data = Encoding.UTF8.GetBytes(message);
                    client.Send(data, data.Length, endpoint);
				}
                catch (System.Net.Sockets.SocketException e)
				{
					System.Console.WriteLine(e.Message);
				}
			}
		}

        public async Task<Received> Receive() {
            var result = await client.ReceiveAsync();
            _connections.Add(result.RemoteEndPoint);
            return new Received
            {
                Message = Encoding.UTF8.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }

    }
}
