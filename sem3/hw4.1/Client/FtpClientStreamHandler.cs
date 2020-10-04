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
            var buffer = new char[long.MaxValue.ToString().Length];
            await CheckIfFileNotFound(buffer);
            var size = await FindSizeOfFile(buffer);
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
        }

        private async Task CheckIfFileNotFound(char[] buffer)
        {
            for (var i  = 0; i < 2; i++)
            {
                await reader.ReadAsync(buffer, i, 1);
            }

            if (buffer[0] == '-' && buffer[1] == '1')
            {
                throw new FileNotFoundException("The specified file was not found.");
            }
        }

        private async Task<long> FindSizeOfFile(char[] buffer) 
        {
            var spaceIndex = 1;
            while (buffer[spaceIndex++] != ' ')
            {
                await reader.ReadAsync(buffer, spaceIndex, 1);
            } 
            
            var stringBuilder = new StringBuilder(spaceIndex);
            for (var i = 0; i < spaceIndex; i++)
            {
                stringBuilder.Append(buffer[i]);
            }

            return long.Parse(stringBuilder.ToString());
        }

        private async Task<char[]> ReadFile(long size)
        {
            var fileContent = new char[size];
            var count = 0L;
            var intermediateStorage = new char[int.MaxValue];
            for (var i = 0; i < size / int.MaxValue; i++)
            {
                await reader.ReadAsync(intermediateStorage, 0, int.MaxValue);

                foreach (var symbol in intermediateStorage)
                {
                    fileContent[count] = symbol;
                    count++;
                }
            }

            await reader.ReadAsync(intermediateStorage, 0, (int) size % int.MaxValue);
            foreach (var symbol in intermediateStorage)
            {
                fileContent[count] = symbol;
                count++;
            }

            return fileContent;
        }
    }
}