using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    internal static class Program
    {
        private static async Task Main()
        {
            const string host = "localhost";
            const int port = 49001;
            var tcpClient = new TcpClient(host, port);
            using var cuiOfFtpClient = new CuiOfFtpClient(
                new FtpClient(new FtpClientStreamHandler(tcpClient.GetStream())),
                Console.Out, Console.In);
            await cuiOfFtpClient.Run();
        }
    }
}