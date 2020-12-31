using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect number of arguments.");
                return;
            }
            
            await new RunnerCli(new Runner(), Console.Out, new AssemblyHandler()).Run(args[0]);
        }
    }
}