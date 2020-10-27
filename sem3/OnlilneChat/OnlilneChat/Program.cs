using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OnlineChat
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            int port;
            switch (args.Length)
            {
                case 2 when IPAddress.TryParse(args[0], out var ipAddress):
                {
                    if (int.TryParse(args[1], out port))
                    {
                        await StartAsServer(ipAddress, port);
                    }

                    break;
                }
                case 2:
                    Console.WriteLine("Incorrect command line arguments.");
                    break;
                case 1 when int.TryParse(args[0], out port):
                    StartAsClient(port);
                    break;
                case 1:
                    Console.WriteLine("Incorrect command line arguments.");
                    break;
            }
        }

        private static async Task StartAsServer(IPAddress ipAddress, int port)
        {
            var server = new Server(ipAddress, port);
            await server.Start();
        }

        private static void StartAsClient(int port)
        {
            var client = new Client(new StreamHandler(new TcpClient(IPAddress.Loopback.ToString(), port).GetStream(),
                Console.In,
                Console.Out));
            client.Start();
        }
    }
}