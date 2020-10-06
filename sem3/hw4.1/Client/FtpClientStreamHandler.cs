using System;
using System.IO;
using System.Threading.Tasks;
using Client.Interfaces;

namespace Client
{
    public class FtpClientStreamHandler : IFtpClientStreamHandler
    {
        private readonly StreamWriter streamWriter;
        private readonly StreamReader streamReader;
        private const int BufferSize = 4096;

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

        public async Task CopyToAsync(Stream destination, long count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count should be not negative.");
            }

            var intermediateStorage = new byte[BufferSize];
            for (var i = 0; i < count / intermediateStorage.Length; i++)
            {
                await streamReader.BaseStream.ReadAsync(intermediateStorage, 0, intermediateStorage.Length);

                await destination.WriteAsync(intermediateStorage);
            }

            await streamReader.BaseStream.ReadAsync(intermediateStorage, 0, (int) count % intermediateStorage.Length);
            await destination.WriteAsync(intermediateStorage, 0, (int) count % intermediateStorage.Length);
        }

        public void Dispose()
        {
            streamWriter.Dispose();
            streamReader.Dispose();
        }
    }
}