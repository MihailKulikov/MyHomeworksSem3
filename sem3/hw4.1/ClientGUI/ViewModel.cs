using Client;
using Client.Interfaces;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientGUI
{
    class ViewModel
    {
        public int Port { get; set; } = 1;
        public string Address { get; set; } = "localhost";

        private IFtpClient ftpClient;

        public ViewModel()
        {
        }

        public async void Connect()
        {
            ftpClient = new FtpClient(
                new FtpClientStreamHandler(await Task.Run(() => new TcpClient(Address, Port).GetStream())));
        }
    }
}
