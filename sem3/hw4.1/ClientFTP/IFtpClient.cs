using System.Threading.Tasks;

namespace ClientFTP
{
    public interface IFtpClient
    {
        public Task<string> MakeRequestAsync(string request);
    }
}