using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communications
{
    public class UdpServer : UdpBase
    {

        protected readonly ISet<IPEndPoint> _connections = new HashSet<IPEndPoint>();

        public UdpServer(int port) : this(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port))
        {
        }

        public UdpServer(IPEndPoint endpoint)
        {
            Client = new System.Net.Sockets.UdpClient(endpoint);
            Port = endpoint.Port;
        }

        public void Send(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

        public new async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            _connections.Add(result.RemoteEndPoint);
            return new Received
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }

        public void SendBroadcast(string message)
        {
            foreach (var endpoint in _connections)
            {
                try
                {
                    Send(message, endpoint);
                }
                catch (SocketException e) {
                    System.Console.WriteLine(e.Message);
                }
            }
        }



    }
}
