using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client.Interfaces;

namespace Client
{
    public class CuiOfFtpClient : IDisposable
    {
        private const string ExitCode = "qa!";
        private const string InputCommandPattern = "^[12] ..*";
        private const string IncorrectInputMessage = "Incorrect input :) Try again.";

        private const string ListCommandInformation =
            "List, request format:\n\t1 <path: String>\n\tpath - path to the directory relative to where the server is running";

        private const string GetCommandInformation =
            "Get, request format:\n\t2 <path: String>\n\tpath - path to the file relative to where the server is running";

        private IEnumerable<string> IntroductionMessages
        {
            get
            {
                yield return "Connection established.";
                yield return ListCommandInformation;
                yield return GetCommandInformation;
                yield return exitCodeInformation;
            }
        }

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
                    await textWriter.WriteLineAsync(IncorrectInputMessage);
                }
                else
                {
                    switch (input[0])
                    {
                        case '1':
                            await ShowListResult(input.Substring(2));

                            break;
                        case '2':
                            await ShowGetResult(input.Substring(2));

                            break;
                    }
                }
            }
        }

        private async Task ShowGetResult(string path)
        {
            try
            {
                await textWriter.WriteLineAsync(
                    $"File successfully downloaded to {await ftpClient.Get(path.Substring(2))}");
            }
            catch (FileNotFoundException e)
            {
                await textWriter.WriteLineAsync(e.Message);
            }
        }

        private async Task ShowListResult(string path)
        {
            try
            {
                var result = await ftpClient.List(path.Substring(2));
                for (var i = 0; i < result.Length; i++)
                {
                    await textWriter.WriteLineAsync($"{result[i].name} {result[i].isDirectory}");
                }
            }
            catch (DirectoryNotFoundException e)
            {
                await textWriter.WriteLineAsync(e.Message);
            }
        }

        private async Task ShowIntroduction()
        {
            foreach (var message in IntroductionMessages)
            {
                await textWriter.WriteLineAsync(message);
            }
        }

        public void Dispose()
        {
            ftpClient.Dispose();
            textWriter.Dispose();
            textReader.Dispose();
        }
    }
}