using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientFTP
{
    public class FtpClient : IFtpClient, IDisposable
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
            return await FtpClientStreamHandler.HandleRequestAsync(stream, request);
        }
        
        public void Dispose()
        {
            tcpClient.Dispose();
            stream.Dispose();
        }
    }
}