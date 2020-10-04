using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class FtpClientStreamHandler : IDisposable
    {
        private readonly StreamWriter writer;
        private readonly StreamReader reader;

        public FtpClientStreamHandler(Stream stream)
        {
            writer = new StreamWriter(stream) {AutoFlush = true};
            reader = new StreamReader(stream);
        }

        public async Task<string> List(string request)
        {
            await writer.WriteLineAsync(request);
            var data = await reader.ReadLineAsync();
            if (data == null || data == "-1")
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }

            return data;
        }

        public async Task<byte[]> Get(string request)
        {
            await writer.WriteLineAsync(request);
            var data = await reader.ReadToEndAsync();
            if (data == "-1")
            {
                throw new FileNotFoundException("The specified file was not found.");
            }

            var splittedData = data.Split(' ');
            var fileContent = Encoding.Default.GetBytes(splittedData[1]);

            return fileContent;
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
        }
    }
}