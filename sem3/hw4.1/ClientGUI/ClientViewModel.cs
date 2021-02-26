using ClientGUI.ItemTemplates;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using ClientGUI.Interfaces;

namespace ClientGUI
{
    public class ClientViewModel
    {
        private const string Root = "./";
        public int Port { get; set; }
        public string Address { get; set; }

        private string currentPath = Root;
        
        private const string ServerConnectionErrorMessage =
            "Server connection error.";

        private const string ErrorMessageBoxCaption = "Error";

        public ObservableCollection<DownloadedFile> DownloadedFiles { get; set; } =
            new ObservableCollection<DownloadedFile>();

        public ObservableCollection<ListResult> ListResults { get; set; } = new ObservableCollection<ListResult>();

        private readonly IFtpClientGui ftpClient;

        public static async Task<ClientViewModel> BuildViewModel(int port, string address, IFtpClientGui ftpClient)
        {
            var viewModel = new ClientViewModel(port, address, ftpClient);
            await viewModel.GetListResults(Root);

            return viewModel;
        }

        private ClientViewModel(int port, string address, IFtpClientGui ftpClient)
        {
            Port = port;
            Address = address;
            this.ftpClient = ftpClient;
        }

        private async Task GetListResults(string path)
        {
            ListResults.Clear();

            if (currentPath != Root) ListResults.Add(new ListResult(true, ".."));
            foreach (var item in (await ftpClient.ListAsync(path))
                .AsParallel()
                .Select(result => new ListResult(result.isDirectory, result.name))
                .OrderBy(listResult => !listResult.IsDirectory))
            {
                ListResults.Add(item);
            }
        }

        public async Task GoTo(string directoryName)
        {
            currentPath = directoryName == ".."
                ? currentPath.Split('/')[..^1].Aggregate("", (acc, item) => acc + (item + '/'))
                : Path.Combine(currentPath, directoryName);
            await GetListResults(currentPath);
        }

        public void Download(string fileName)
        {
            var downloadFile = new DownloadedFile(fileName, 0);
            DownloadedFiles.Add(downloadFile);
            Task.Run(async () =>
            {
                TcpClient tcpClient;
                try
                {
                    tcpClient = new TcpClient(Address, Port);
                }
                catch
                {
                    MessageBox.Show(ServerConnectionErrorMessage, ErrorMessageBoxCaption);
                    return;
                }
                
                await new FtpClientGui(new FtpClientStreamHandlerGui(tcpClient.GetStream())).GetAsync(fileName,
                    progressPercentage => downloadFile.Completion = progressPercentage);
            });
        }

        public void DownloadAll()
        {
            foreach (var listResult in ListResults.Where(result => !result.IsDirectory))
            {
                Download(listResult.Name);
            }
        }
    }
}