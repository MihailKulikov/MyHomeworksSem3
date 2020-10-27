using System;
using System.IO;
using System.Threading.Tasks;

namespace OnlineChat
{
    public class StreamHandler : IDisposable
    {
        private readonly Stream stream;
        private readonly TextReader textReader;
        private readonly TextWriter textWriter;

        public StreamHandler(Stream stream, TextReader textReader, TextWriter textWriter)
        {
            this.stream = stream;
            this.textReader = textReader;
            this.textWriter = textWriter;
        }

        public async Task WriteToStream()
        {
            await using var streamWriter = new StreamWriter(stream) {AutoFlush = true};
            while (true)
            {
                var data = await textReader.ReadLineAsync();
                await streamWriter.WriteLineAsync(data);
                if (data == "exit")
                {
                    break;
                }
            }
        }

        public async Task ReadFromStream()
        {
            using var streamReader = new StreamReader(stream);
            while (true)
            {
                var data = await streamReader.ReadLineAsync();
                await textWriter.WriteLineAsync(data);
                if (data == "exit")
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            stream.Dispose();
            textReader.Dispose();
            textWriter.Dispose();
        }
    }
}