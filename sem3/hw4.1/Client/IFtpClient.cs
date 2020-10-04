using System;
using System.Threading.Tasks;

namespace Client
{
    public interface IFtpClient : IDisposable
    {
        public Task<(string name, bool isDirectory)[]> List(string path);
        public Task<string> Get(string path);
    }
}