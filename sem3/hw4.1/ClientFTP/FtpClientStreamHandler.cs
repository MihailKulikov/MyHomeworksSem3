using System.IO;
using System.Threading.Tasks;

namespace ClientFTP
{
    public static class FtpClientStreamHandler
    {
        public static async Task<string> HandleRequestAsync(Stream stream, string request)
        {
            var writer = new StreamWriter(stream); 
            await writer.WriteLineAsync(request);
            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            return (string) data;
        }
    }
}