using System;
using System.IO;
using System.Threading.Tasks;
using Client;
using ClientGUI.Interfaces;

namespace ClientGUI
{
    public class FtpClientGui : FtpClient, IFtpClientGui
    {
        public FtpClientGui(IFtpClientStreamHandlerGui streamHandler) : base(streamHandler)
        {
        }

        public async Task<string> GetAsync(string path, Action<int> updateProgressPercentageAction)
        {
            var request = $"2 {path}";
            await streamHandler.WriteLineAsync(request);

            var size = await FindSizeOfFile();
            if (size == -1)
            {
                throw new FileNotFoundException("The specified file was not found.");
            }

            return await DownloadFile(size, path, updateProgressPercentageAction);
        }

        private async Task<string> DownloadFile(long size, string path, Action<int> updateProgressPercentageAction)
        {
            if (!Directory.Exists(PathToDownloadsDirectory))
            {
                Directory.CreateDirectory(PathToDownloadsDirectory);
            }

            var fileName = @$"{PathToDownloadsDirectory}\{Path.GetFileName(path)}";
            await using var fileStream = File.Create(fileName);
            await ((IFtpClientStreamHandlerGui) streamHandler).CopyToAsync(fileStream, size,
                updateProgressPercentageAction);

            return fileName;
        }
    }
}