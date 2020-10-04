using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    public class CuiOfFtpClient : IDisposable
    {
        private const string ExitCode = "qa!";
        private const string InputCommandPattern = "^[12] ..*";
        private const string ListCommandInformation =
            "List, request format:\n\t1 <path: String>\n\tpath - path to the directory relative to where the server is running";
        private const string GetCommandInformation =
            "Get, request format:\n\t2 <path: String>\n\tpath - path to the file relative to where the server is running";

        private readonly string exitCodeInformation = $"Exit, request format:\n\t{ExitCode}";
        private readonly IFtpClient ftpClient;
        private readonly TextWriter textWriter;
        private readonly TextReader textReader;

        public CuiOfFtpClient(IFtpClient ftpClient, TextWriter textWriter, TextReader textReader)
        {
            this.ftpClient = ftpClient;
            this.textWriter = textWriter;
            this.textReader = textReader;
        }

        public async Task Run()
        {
            await ShowIntroduction();
            while (true)
            {
                var input = await textReader.ReadLineAsync();
                if (input == ExitCode)
                {
                    break;
                }

                if (input == null || !Regex.IsMatch(input, InputCommandPattern))
                {
                    await textWriter.WriteLineAsync("Incorrect input :) Try again.");
                }

                await textWriter.WriteLineAsync(await ftpClient.MakeRequestAsync(input!));
            }
        }

        private async Task ShowIntroduction()
        {
            await textWriter.WriteLineAsync("Connection established.");
            await textWriter.WriteLineAsync(ListCommandInformation);
            await textWriter.WriteLineAsync(GetCommandInformation);
            await textWriter.WriteLineAsync(exitCodeInformation);
        }

        public void Dispose()
        {
            ftpClient.Dispose();
            textWriter.Dispose();
            textReader.Dispose();
        }
    }
}