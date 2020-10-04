﻿using System;
using System.IO;
using System.Text;
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
                else
                {
                    switch (input[0])
                    {
                        case '1':
                            try
                            {
                                var result = await ftpClient.List(input.Substring(2));
                                for (var i = 0; i < result.Length; i++)
                                {
                                    await textWriter.WriteAsync($"{result[i].name} {result[i].isDirectory}");
                                    if (i != result.Length - 1)
                                    {
                                        await textWriter.WriteAsync(" ");
                                    }
                                    await textWriter.WriteLineAsync();
                                }
                            }
                            catch (DirectoryNotFoundException)
                            {
                                await textWriter.WriteLineAsync("Directory not found.");
                            }
                            break;
                        case '2':
                            try 
                            {
                                await textWriter.WriteLineAsync(await ftpClient.Get(input.Substring(2)));
                            }
                            catch (FileNotFoundException)
                            {
                                await textWriter.WriteLineAsync("File not found.");
                            }
                            break;
                    }
                }
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