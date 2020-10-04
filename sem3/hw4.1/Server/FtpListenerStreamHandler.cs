using System;
using System.IO;
using System.Threading.Tasks;

namespace Server
{
    public static class FtpListenerStreamHandler
    {
        public static async Task HandleStreamAsync(Stream stream)
        {
            while (true)
            {
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream) {AutoFlush = true};
                var request = await reader.ReadLineAsync();
                foreach (var symbol in await FtpRequestHandler.HandleRequest(request))
                {
                    await writer.WriteAsync(Convert.ToChar(symbol));
                }
            }
        }
    }
}