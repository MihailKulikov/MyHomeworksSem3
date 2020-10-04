using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class FtpClient : IFtpClient
    {
        private readonly TcpClient tcpClient;
        private readonly FtpClientStreamHandler ftpClientStreamHandler;

        public FtpClient(string host, int port)
        {
            tcpClient = new TcpClient(host, port);
            ftpClientStreamHandler = new FtpClientStreamHandler(tcpClient.GetStream());
        }

        public async Task<string> List(string path)
            => await ftpClientStreamHandler.List($"1 {path}");

        public async Task<byte[]> Get(string path)
            => await ftpClientStreamHandler.Get($"2 {path}");
        
        public void Dispose()
        {
            tcpClient.Dispose();
            ftpClientStreamHandler.Dispose();
        }
    }
}