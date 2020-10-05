using System;
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
                var client = await tcpListener.AcceptTcpClientAsync();
                var ftpListenerStreamHandler = new FtpListenerRequestHandler(client.GetStream());
                Task.Run(async () =>
                {
                    try
                    {
                        await ftpListenerStreamHandler.HandleStreamAsync();
                    }
                    catch (Exception)
                    {
                        ftpListenerStreamHandler.Dispose();
                    }
                });
            }
        }
    }
}