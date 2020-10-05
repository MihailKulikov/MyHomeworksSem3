using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    public class FtpClientStreamHandler : IDisposable
    {
        private readonly StreamWriter writer;
        private readonly StreamReader reader;
        private const int BufferSize = 4096;
        private const string PatternForSplittingListResponse = "( true )|( false )|( true)|( false)";

        public FtpClientStreamHandler(Stream stream)
        {
            writer = new StreamWriter(stream) {AutoFlush = true};
            reader = new StreamReader(stream);
        }

        public async Task<(string name, bool isDirectory)[]> List(string request)
        {
            await writer.WriteLineAsync(request);
            var data = await reader.ReadLineAsync();
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

        public async Task<string> Get(string request)
        {
            await writer.WriteLineAsync(request);

           
            var size = await FindSizeOfFile();
            if (size == -1)
            {
                throw new FileNotFoundException();
            }
            
            return await DownloadFile(size);
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
        }

        private async Task<long> FindSizeOfFile()
        {
            var buffer = new byte[long.MaxValue.ToString().Length + 1];
            var spaceIndex = 0;
            do
            {
                await reader.BaseStream.ReadAsync(buffer, spaceIndex, 1);
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
            var intermediateStorage = new byte[BufferSize];
            for (var i = 0; i < size / intermediateStorage.Length; i++)
            {
                await reader.BaseStream.ReadAsync(intermediateStorage, 0, intermediateStorage.Length);

                await fileStream.WriteAsync(intermediateStorage);
            }

            await reader.BaseStream.ReadAsync(intermediateStorage, 0, (int) size % intermediateStorage.Length);
            await fileStream.WriteAsync(intermediateStorage, 0, (int) size % intermediateStorage.Length);

            return fileName;
        }
    }
}