using System.Text;
using System.Threading.Tasks;

namespace Communications
{
    public abstract class UdpBase
    {
        protected System.Net.Sockets.UdpClient Client;

        protected UdpBase()
        {
            Client = new System.Net.Sockets.UdpClient();
        }

        protected UdpBase(int port)
        {
            Client = new System.Net.Sockets.UdpClient(port);
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }
}