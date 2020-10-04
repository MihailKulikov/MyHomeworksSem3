using System;
using System.Threading.Tasks;

namespace Client
{
    public interface IFtpClient : IDisposable
    {
        public Task<string> List(string path);
        public Task<byte[]> Get(string path);
    }
}