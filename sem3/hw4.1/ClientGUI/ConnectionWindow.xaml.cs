using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow
    {
        private const string IncorrectPortMessage = "The port should be integer from 0 to 65535.";

        private const string ServerConnectionErrorMessage =
            "Server connection error. Check the correctness of the data you entered.";

        private const string ErrorMessageBoxCaption = "Error";

        public int Port { get; set; } = 49001;
        public string Address { get; set; } = "localhost";

        public ConnectionWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!int.TryParse(PortBox.Text, out var port))
            {
                MessageBox.Show(IncorrectPortMessage, ErrorMessageBoxCaption);
                return;
            }

            var address = AddressBox.Text;
            TcpClient tcpClient;
            try
            {
                tcpClient = await Task.Run(() => new TcpClient(address, port));
            }
            catch
            {
                MessageBox.Show(ServerConnectionErrorMessage, ErrorMessageBoxCaption);
                return;
            }

            var ftpClient = new FtpClient(new FtpClientStreamHandlerGui(tcpClient.GetStream()));
            var workingWindow = new WorkingWindow(await ClientViewModel.BuildViewModel(port, address, ftpClient));
            workingWindow.Show();
        }

        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AddressBox.Text.Length != 0 && PortBox.Text.Length != 0;
        }
    }
}