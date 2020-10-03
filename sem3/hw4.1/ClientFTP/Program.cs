using System;
using System.Threading.Tasks;

namespace ClientFTP
{
    internal static class Program
    {
        private static async Task Main()
        {
            const string host = "localhost";
            const int port = 49001;
            using var ftpClient = new FtpClient(host, port);
            var cuiOfFtpClient = new CuiOfFtpClient(ftpClient, Console.Out, Console.In);
            await cuiOfFtpClient.Run();
        }
    }
}