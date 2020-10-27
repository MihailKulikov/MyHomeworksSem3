using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChat
{
    /// <summary>
    /// Represents server for online chat.
    /// </summary>
    public class Server
    {
        private readonly TcpListener tcpListener;

        /// <summary>
        /// Initialize new instance of the <see cref="Server"/> class with specified Ip address and port.
        /// </summary>
        /// <param name="ipAddress">Specified ip address.</param>
        /// <param name="port">Specified port.</param>
        public Server(IPAddress ipAddress, int port)
        {
            tcpListener = new TcpListener(ipAddress, port);
        }

        /// <summary>
        /// Starts server for reading and writing.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Start()
        {
            tcpListener.Start();
            try
            {
                using var client = await tcpListener.AcceptTcpClientAsync();
                using var streamHandler = new StreamHandler(client.GetStream(), Console.In, Console.Out);
                var cancellationTokenSource = new CancellationTokenSource();
                var writeToStreamTask = Task.Run(async () => await streamHandler.WriteToStream(),
                    cancellationTokenSource.Token);
                var readFromStreamTask = Task.Run(async () => await streamHandler.ReadFromStream(),
                    cancellationTokenSource.Token);
                Task.WaitAny(writeToStreamTask, readFromStreamTask);
                cancellationTokenSource.Cancel();
            }
            catch
            {
                //ignored
            }

            tcpListener.Stop();
        }
    }
}