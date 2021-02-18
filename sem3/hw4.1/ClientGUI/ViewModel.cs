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
        private const string root = "./"; 
        public int Port { get; set; } 
        public string Address { get; set; }

        private string currentPath = root;

        public ObservableCollection<ListResult> ListResults { get; set; } = new ObservableCollection<ListResult>();

        private readonly IFtpClient ftpClient;

        public static async Task<ViewModel> BuildViewModel(int port, string address, IFtpClient ftpClient)
        {
            var viewModel = new ViewModel(port, address, ftpClient);
            await viewModel.GetListResults(root);

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
            for (var i = ListResults.Count - 1; i >= 0; i--)
            {
                ListResults.RemoveAt(i);
            }

            if (currentPath != root) ListResults.Add(new ListResult(true, ".."));
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
            currentPath = directoryName == ".." ? currentPath.Split('/')[0..^1].Aggregate("", (acc, item) => acc += item + '/') : Path.Combine(currentPath, directoryName);
            await GetListResults(currentPath);
        }
    }
}