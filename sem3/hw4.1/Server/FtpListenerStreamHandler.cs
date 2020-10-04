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
                await writer.WriteLineAsync(await FtpRequestHandler.HandleRequest(request));
            }
        }
    }
}