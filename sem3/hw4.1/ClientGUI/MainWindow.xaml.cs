using Client;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string IncorrectPortMessage = "The port should be integer from 0 to 65535.";
        private const string ServerConnectionErrorMessage = "Server connection error. Check the correctness of the data you entered.";
        private const string ErrorMessageBoxCaption = "Error";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnConnection(object sender, EventArgs args)
        {
        }

        private async void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!int.TryParse(Port.Text, out var port))
            {
                MessageBox.Show(IncorrectPortMessage, ErrorMessageBoxCaption);
                return;
            }
            var address = Address.Text;
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
            var win = new MainPage(await ViewModel.BuildViewModel(port, address, new FtpClient(new FtpClientStreamHandler(tcpClient.GetStream()))));
            win.Show();
        }

        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Address.Text.Length != 0 && Port.Text.Length != 0;
        }
    }
}