using System;
using System.IO;
using System.Threading.Tasks;

namespace OnlineChat
{
    /// <summary>
    /// Providing methods for writing stream content to TextWriter and TextReader content to stream.
    /// </summary>
    public class StreamHandler : IDisposable
    {
        private readonly Stream stream;
        private readonly TextReader textReader;
        private readonly TextWriter textWriter;

        /// <summary>
        /// Initialize new instance of the <see cref="StreamHandler"/> class that working with specified stream, textReader and textWriter.
        /// </summary>
        /// <param name="stream">Specified stream.</param>
        /// <param name="textReader">Specified textReader.</param>
        /// <param name="textWriter">Specified textWriter.</param>
        public StreamHandler(Stream stream, TextReader textReader, TextWriter textWriter)
        {
            this.stream = stream;
            this.textReader = textReader;
            this.textWriter = textWriter;
        }

        /// <summary>
        /// Writes content of textReader to the stream.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task WriteToStream()
        {
            var streamWriter = new StreamWriter(stream) {AutoFlush = true};
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

        /// <summary>
        /// Reads content from stream and writes it to textWriter.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReadFromStream()
        {
            var streamReader = new StreamReader(stream);
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

        /// <summary>
        /// Releases all resources used by <see cref="StreamHandler"/> object.
        /// </summary>
        public void Dispose()
        {
            stream.Dispose();
            textReader.Dispose();
            textWriter.Dispose();
        }
    }
}