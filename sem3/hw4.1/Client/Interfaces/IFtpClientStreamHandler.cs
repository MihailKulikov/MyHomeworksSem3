using System;
using System.IO;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IFtpClientStreamHandler : IDisposable
    {
        public Task WriteLineAsync(string input);
        public Task<string?> ReadLineAsync();
        public Task<int> ReadAsync(byte[] buffer, int offset, int count);
        public Task CopyToAsync(Stream destination, long count);
    }
}