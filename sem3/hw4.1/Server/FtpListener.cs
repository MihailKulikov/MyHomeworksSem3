using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Listens for connection from SimpleFTP clients.
    /// </summary>
    public class FtpListener
    {
        private readonly TcpListener tcpListener;

        /// <summary>
        /// Initialize a new instance of the <see cref="FtpListener"/> class that listens for incoming connection attempts on the specified local IP address and port number.
        /// </summary>
        /// <param name="ipAddress">An <see cref="IPAddress"/> that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        public FtpListener(IPAddress ipAddress, int port)
        {
            tcpListener = new TcpListener(ipAddress, port);
        }

        /// <summary>
        /// Starts listening for incoming connection requests and process them in another thread on a thread pool.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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