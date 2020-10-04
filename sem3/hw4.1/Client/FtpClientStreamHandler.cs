using System.IO;
using System.Threading.Tasks;

namespace Client
{
    public static class FtpClientStreamHandler
    {
        public static async Task<string> HandleStreamAsync(Stream stream, string request)
        {
            //TODO: Different handling for Get and List
            await using var writer = new StreamWriter(stream) {AutoFlush = true}; 
            await writer.WriteLineAsync(request);
            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            return data ?? "-1";
        }
    }
}