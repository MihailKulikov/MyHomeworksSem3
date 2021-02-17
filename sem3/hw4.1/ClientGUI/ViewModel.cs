using Client;
using Client.Interfaces;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientGUI
{
    public class ViewModel
    {
        public int Port { get; set; } 
        public string Address { get; set; }

        private IFtpClient ftpClient;

        public ViewModel(int port, string adress, IFtpClient ftpClient)
        {
            Port = port;
            Address = adress;
            this.ftpClient = ftpClient;
        }
    }
}
