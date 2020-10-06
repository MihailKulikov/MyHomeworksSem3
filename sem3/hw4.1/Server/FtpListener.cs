using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class FtpListener
    {
        private readonly TcpListener tcpListener;

        public FtpListener(IPAddress ipAddress, int port)
        {
            tcpListener = new TcpListener(ipAddress, port);
        }

        public async Task Start()
        {
            tcpListener.Start();
            while (true)
            {
                try
                {
                    var client = await tcpListener.AcceptTcpClientAsync();
                    Task.Run(async () => await HandleRequests(client.GetStream()));
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static async Task HandleRequests(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var ftpListenerRequestHandler = new FtpListenerRequestHandler(stream);
            while (true)
            {
                try
                {
                    var request = await reader.ReadLineAsync();
                    if (request != null)
                    {
                        await ftpListenerRequestHandler.HandleRequestAsync(request);
                    }
                }
                catch
                {
                    ftpListenerRequestHandler.Dispose();
                    break;
                }
            }
        }
    }
}