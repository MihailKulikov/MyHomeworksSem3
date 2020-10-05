using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client.Interfaces;

namespace Client
{
    public class FtpClient : IFtpClient
    {
        private readonly IFtpClientStreamHandler streamHandler;
        private const string PatternForSplittingListResponse = "( true )|( false )|( true)|( false)";

        public FtpClient(IFtpClientStreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

        public void Dispose()
        {
            streamHandler.Dispose();
        }

        public async Task<(string name, bool isDirectory)[]> List(string path)
        {
            var request = $"1 {path}";
            await streamHandler.WriteLineAsync(request);
            var data = await streamHandler.ReadLineAsync();
            if (data == null || data == "-1")
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }

            var splittedData = data.Split(' ', 2);
            var size = int.Parse(splittedData[0]);
            var nameAndIsDirStrings = Regex.Split(splittedData[1], PatternForSplittingListResponse);
            var result = new (string name, bool isDirectory)[size];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (nameAndIsDirStrings[i * 2], bool.Parse(nameAndIsDirStrings[i * 2 + 1]));
            }

            return result;
        }
        
        public async Task<string> Get(string path)
        {
            var request = $"2 {path}";
            await streamHandler.WriteLineAsync(request);
            
            var size = await FindSizeOfFile();
            if (size == -1)
            {
                throw new FileNotFoundException();
            }
            
            return await DownloadFile(size);
        }
        
        private async Task<long> FindSizeOfFile()
        {
            var buffer = new byte[long.MaxValue.ToString().Length + 1];
            var spaceIndex = 0;
            do
            {
                await streamHandler.ReadAsync(buffer, spaceIndex, 1);
            } while (buffer[spaceIndex++] != ' ');

            spaceIndex--;

            var stringBuilder = new StringBuilder(spaceIndex);
            for (var i = 0; i < spaceIndex; i++)
            {
                stringBuilder.Append(Convert.ToChar(buffer[i]));
            }

            return long.Parse(stringBuilder.ToString());
        }

        private async Task<string> DownloadFile(long size)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            await using var fileStream = File.Create(fileName);
            await streamHandler.CopyToAsync(fileStream, size);

            return fileName;
        }
    }
}