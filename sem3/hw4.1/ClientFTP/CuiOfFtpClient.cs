using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClientFTP
{
    public class CuiOfFtpClient
    {
        private const string ExitCode = "qa!";
        private const string InputCommandPattern = "^[12] ..*";
        private readonly IFtpClient ftpClient;
        private readonly TextWriter textWriter;
        private readonly TextReader textReader;

        public CuiOfFtpClient(IFtpClient ftpClient, TextWriter textWriter, TextReader textReader)
        {
            this.ftpClient = ftpClient ?? throw new ArgumentNullException(nameof(ftpClient));
            this.textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            this.textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
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

                await textWriter.WriteLineAsync(await ftpClient.MakeRequestAsync(input));
            }
        }

        private async Task ShowIntroduction()
        {
            await textWriter.WriteLineAsync("Connection established.");
            await textWriter.WriteLineAsync(
                "List, request format:\n\t1 <path: String>\n\tpath - path to the directory relative to where the server is running");
            await textWriter.WriteLineAsync(
                "Get, request format:\n\t2 <path: String>\n\tpath - path to the file relative to where the server is running");
            await textWriter.WriteLineAsync($"Exit, request format:\n\t{ExitCode}");
        }
    }
}