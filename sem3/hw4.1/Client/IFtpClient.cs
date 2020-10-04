using System;
using System.Threading.Tasks;

namespace Client
{
    public interface IFtpClient : IDisposable
    {
        public Task<string> MakeRequestAsync(string request);
    }
}