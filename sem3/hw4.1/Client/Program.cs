﻿using System;
using System.Threading.Tasks;

namespace Client
{
    internal static class Program
    {
        private static async Task Main()
        {
            const string host = "localhost";
            int dfg = 0X7FFFFFC7;
            const int port = 49001;
            using var cuiOfFtpClient = new CuiOfFtpClient(new FtpClient(host, port), Console.Out, Console.In);
            await cuiOfFtpClient.Run();
        }
    }
}