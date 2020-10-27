using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChat
{
    public class Server
    {
        private readonly TcpListener tcpListener;

        public Server(IPAddress ipAddress, int port)
        {
            tcpListener = new TcpListener(ipAddress, port);
        }

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