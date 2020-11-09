using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;

namespace MyNUnit.Runner
{
    public class RunnerCli
    {
        private const string IntroduceMessage = "Enter the path to the assemblies:";

        private const string ExceptionMessage =
            "The following exceptions were thrown in the After, AfterClass, Before, BeforeClass blocks";
        private readonly IRunner runner;
        private readonly TextWriter textWriter;
        private readonly TextReader textReader;
        private readonly IAssemblyHandler assemblyHandler;
        
        public RunnerCli(IRunner runner, TextWriter textWriter, TextReader textReader, IAssemblyHandler assemblyHandler)
        {
            this.runner = runner;
            this.textWriter = textWriter;
            this.textReader = textReader;
            this.assemblyHandler = assemblyHandler;
        }

        public async Task Run()
        {
            await textWriter.WriteLineAsync(IntroduceMessage);
            string path = (await textReader.ReadLineAsync())!;
            await WriteResults(runner.RunTests(assemblyHandler.GetTestClassesFromAssemblies(path)));
        }

        private async Task WriteResults(IEnumerable<TestResult> results)
        {
            foreach (var result in results)
            {
                await textWriter.WriteLineAsync(result.ClassName);
                await textWriter.WriteLineAsync(ExceptionMessage);
                foreach (var exception in result.Exceptions)
                {
                    Console.WriteLine(exception.ToString());
                }

                foreach (var testMethod in result.TestMethods)
                {
                    await WriteTestMethodInfo(testMethod);
                }
            }
        }

        private async Task WriteTestMethodInfo(ITestMethod testMethod)
        {
            switch (testMethod)
            {
                case IgnoredTestMethod ignoredTestMethod:
                    await textWriter.WriteLineAsync(
                        $"{ignoredTestMethod.Name} was ignored because of {ignoredTestMethod.ReasonForIgnoring}");
                    break;
                case FailedTestMethod failedTestMethod:
                {
                    if (failedTestMethod.ExpectedExceptionType != null)
                    {
                        await textWriter.WriteLineAsync(
                            $"{failedTestMethod.Name} {failedTestMethod.ElapsedTime}: expected {failedTestMethod.ExpectedExceptionType}," +
                            $" but was {failedTestMethod.ThrownException.GetType()}");
                    }
                    else
                    {
                        await textWriter.WriteLineAsync(
                            $"{failedTestMethod.Name}: {failedTestMethod.ThrownException.GetType()} was thrown.");
                    }
                
                    await textWriter.WriteLineAsync($"Stack trace: {failedTestMethod.ThrownException.StackTrace}");
                    break;
                }
                case SuccessfulTestMethod successfulTestMethod:
                    await textWriter.WriteLineAsync(
                        $"{successfulTestMethod.Name} was successful within {successfulTestMethod.ElapsedTime}");
                    break;
            }
        }
    }
}