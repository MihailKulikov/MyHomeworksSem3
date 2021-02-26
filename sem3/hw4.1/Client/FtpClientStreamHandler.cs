using System;
using System.IO;
using System.Threading.Tasks;
using Client.Interfaces;

namespace Client
{
    /// <summary>
    /// Represents implementation of a <see cref="IFtpClientStreamHandler"/>.
    /// </summary>
    public class FtpClientStreamHandler : IFtpClientStreamHandler
    {
        private readonly StreamWriter streamWriter;
        private readonly StreamReader streamReader;
        private const int BufferSize = 4096;

        /// <summary>
        /// Initialize new instance of the <see cref="FtpClientStreamHandler"/> class that works with specified stream.
        /// </summary>
        /// <param name="stream">Specified stream.</param>
        public FtpClientStreamHandler(Stream stream)
        {
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) {AutoFlush = true};
        }

        public async Task WriteLineAsync(string input)
            => await streamWriter.WriteLineAsync(input);

        public async Task<string?> ReadLineAsync()
            => await streamReader.ReadLineAsync();

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
            => await streamReader.BaseStream.ReadAsync(buffer, offset, count);

        public virtual async Task CopyToAsync(Stream destination, long count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count should be not negative.");
            }

            var intermediateStorage = new byte[BufferSize];
            for (var i = 0; i < count / BufferSize; i++)
            {
                await WriteFromStreamToStreamUsingBuffer(intermediateStorage, intermediateStorage.Length, destination);
            }

            await WriteFromStreamToStreamUsingBuffer(intermediateStorage, (int) count % intermediateStorage.Length,
                destination);
        }

        /// <summary>
        /// Release all resources used by <see cref="FtpClientStreamHandler"/> object.
        /// </summary>
        public void Dispose()
        {
            streamReader.BaseStream.Dispose();
            streamWriter.Dispose();
            streamReader.Dispose();
        }

        protected async Task WriteFromStreamToStreamUsingBuffer(byte[] intermediateStorage, int count, Stream destination)
        {
            await streamReader.BaseStream.ReadAsync(intermediateStorage, 0, count);
            await destination.WriteAsync(intermediateStorage, 0, count);
        }
    }
}