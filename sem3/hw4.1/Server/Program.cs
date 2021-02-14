using System.Net;
using System.Threading.Tasks;

namespace Server
{
    internal static class Program
    {
        private static async Task Main()
        {
            const int port = 49001;
            var ftpListener = new FtpListener(IPAddress.Loopback, port);
            await ftpListener.Start();
        }
    }
}