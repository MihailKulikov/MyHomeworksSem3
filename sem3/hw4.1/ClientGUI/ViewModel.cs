using Client.Interfaces;
using ClientGUI.ItemTemplates;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace ClientGUI
{
    public class ViewModel
    {
        private const string Root = "./";
        public int Port { get; set; }
        public string Address { get; set; }

        private string currentPath = Root;

        public ObservableCollection<ListResult> ListResults { get; set; } = new ObservableCollection<ListResult>();

        private readonly IFtpClient ftpClient;

        public static async Task<ViewModel> BuildViewModel(int port, string address, IFtpClient ftpClient)
        {
            var viewModel = new ViewModel(port, address, ftpClient);
            await viewModel.GetListResults(Root);

            return viewModel;
        }

        private ViewModel(int port, string address, IFtpClient ftpClient)
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

        public async Task Download(string fileName)
        {
            
        }

        public async Task DownloadAll()
        {
            
        }
    }
}