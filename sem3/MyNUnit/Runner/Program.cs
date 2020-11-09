using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner
{
    internal static class Program
    {
        private static async Task Main()
        {
            await new RunnerCli(new Runner(), Console.Out, Console.In, new AssemblyHandler()).Run();
        }
    }
}