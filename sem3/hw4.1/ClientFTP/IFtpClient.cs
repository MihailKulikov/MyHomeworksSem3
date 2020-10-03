using System;
using System.Threading.Tasks;

namespace ClientFTP
{
    public interface IFtpClient : IDisposable
    {
        public Task<string> MakeRequestAsync(string request);
    }
}