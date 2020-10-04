using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class FtpClient : IFtpClient
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;
        
        public FtpClient(string host, int port)
        {
            tcpClient = new TcpClient(host, port);
            stream = tcpClient.GetStream();
        }

        public async Task<string> MakeRequestAsync(string request)
        {
            return await FtpClientStreamHandler.HandleStreamAsync(stream, request);
        }
        
        public void Dispose()
        {
            tcpClient.Dispose();
            stream.Dispose();
        }
    }
}