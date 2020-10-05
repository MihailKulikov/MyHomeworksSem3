using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class FtpClientStreamHandler : IDisposable
    {
        private readonly StreamWriter writer;
        private readonly StreamReader reader;
        private const int MaxBufferLength = 4096;

        public FtpClientStreamHandler(Stream stream)
        {
            writer = new StreamWriter(stream) {AutoFlush = true};
            reader = new StreamReader(stream);
        }

        public async Task<(string name, bool isDirectory)[]> List(string request)
        {
            //TODO: SHOULD WORK WITH NAMES WITH SPACES
            await writer.WriteLineAsync(request);
            var data = await reader.ReadLineAsync();
            if (data == null || data == "-1")
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }
            var splittedData = data.Split(' ');
            var size = int.Parse(splittedData[0]);
            var result = new (string name, bool isDirectory)[size];
            var resultCounter = 0;
            while (resultCounter < result.Length)
            {
                result[resultCounter] = (splittedData[resultCounter * 2 + 1], bool.Parse(splittedData[resultCounter * 2 + 2]));
                resultCounter++;
            }

            return result;
        }

        public async Task<string> Get(string request)
        {
            await writer.WriteLineAsync(request);
            
            var buffer = new byte[long.MaxValue.ToString().Length];
            await CheckIfFileNotFound(buffer);
            return await DownloadFile(await FindSizeOfFile(buffer));
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
        }

        private async Task CheckIfFileNotFound(byte[] buffer)
        {
            for (var i  = 0; i < 2; i++)
            {
                await reader.BaseStream.ReadAsync(buffer, i, 1);
            }

            if (buffer[0] == '-' && buffer[1] == '1')
            {
                await reader.ReadLineAsync();
                throw new FileNotFoundException("The specified file was not found.");
            }
        }

        private async Task<long> FindSizeOfFile(byte[] buffer) 
        {
            var spaceIndex = 1;
            while (buffer[spaceIndex] != ' ')
            {
                spaceIndex++;
                await reader.BaseStream.ReadAsync(buffer, spaceIndex, 1);
            } 
            
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
            var intermediateStorage = new byte[MaxBufferLength];
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