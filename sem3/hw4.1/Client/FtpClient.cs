using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client.Interfaces;

namespace Client
{
    /// <summary>
    /// Represents implementation of a <see cref="IFtpClient"/>.
    /// </summary>
    public class FtpClient : IFtpClient
    {
        private const string PathToDownloadsDirectory = "SimpleFtpDownloads";
        private readonly IFtpClientStreamHandler streamHandler;
        private readonly string patternForSplittingListResponse = $"( {true} )|( {false} )|( {true})|( {false})";

        /// <summary>
        /// Initialize a new instance of the <see cref="FtpClient"/> class with specified <see cref="IFtpClientStreamHandler"/>.
        /// </summary>
        /// <param name="streamHandler">Specified <see cref="IFtpClientStreamHandler"/> for working with stream.</param>
        public FtpClient(IFtpClientStreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

        /// <summary>
        /// Releases all resources used by <see cref="FtpClient"/> object.
        /// </summary>
        public void Dispose()
        {
            streamHandler.Dispose();
        }

        /// <summary>
        /// Makes file listing in the directory with specified path on the server.
        /// </summary>
        /// <param name="path">Path to the directory relative to where the server is running.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the sequence of file's names and flags
        /// indicating whether the file is a directory.</returns>
        /// <exception cref="DirectoryNotFoundException">The directory with specified path was not found.</exception>
        public async Task<IEnumerable<(string name, bool isDirectory)>> ListAsync(string path)
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
            var nameAndIsDirStrings = Regex.Split(splittedData[1], patternForSplittingListResponse);
            var result = new (string name, bool isDirectory)[size].Select((tuple, i) =>
                (nameAndIsDirStrings[i * 2], bool.Parse(nameAndIsDirStrings[i * 2 + 1])));

            return result;
        }

        /// <summary>
        /// Downloads file from server.
        /// </summary>
        /// <param name="path">Path to the file relative to where the server is running.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains path to the downloaded file relative to where the client is running.</returns>
        /// <exception cref="FileNotFoundException">The file with specified path was not found.</exception>
        public async Task<string> GetAsync(string path)
        {
            var request = $"2 {path}";
            await streamHandler.WriteLineAsync(request);

            var size = await FindSizeOfFile();
            if (size == -1)
            {
                throw new FileNotFoundException("The specified file was not found.");
            }

            return await DownloadFile(size, path);
        }

        private async Task<long> FindSizeOfFile()
        {
            var buffer = new byte[long.MaxValue.ToString().Length + 1];
            await streamHandler.ReadAsync(buffer, 0, 2);

            if (buffer[0] == '-' && buffer[1] == '1')
            {
                return -1;
            }

            var spaceIndex = 1;
            while (buffer[spaceIndex] != ' ')
            {
                spaceIndex++;
                await streamHandler.ReadAsync(buffer, spaceIndex, 1);
            }

            var stringBuilder = new StringBuilder(spaceIndex);
            for (var i = 0; i < spaceIndex; i++)
            {
                stringBuilder.Append(Convert.ToChar(buffer[i]));
            }

            return long.Parse(stringBuilder.ToString());
        }

        private async Task<string> DownloadFile(long size, string path)
        {
            if (!Directory.Exists(PathToDownloadsDirectory))
            {
                Directory.CreateDirectory(PathToDownloadsDirectory);
            }

            var fileName = @$"{PathToDownloadsDirectory}\{Path.GetFileName(path)}";
            await using var fileStream = File.Create(fileName);
            await streamHandler.CopyToAsync(fileStream, size);
    
            return fileName;
        }
    }
}