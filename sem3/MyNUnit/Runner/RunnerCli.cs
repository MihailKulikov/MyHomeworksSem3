﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents CLI for Runner.
    /// </summary>
    public class RunnerCli
    {
        private const string IntroduceMessage = "Enter the path to the assemblies:";

        private const string ExceptionMessage =
            "The following exceptions were thrown in the After, AfterClass, Before, BeforeClass blocks";

        private readonly IRunner runner;
        private readonly TextWriter textWriter;
        private readonly TextReader textReader;
        private readonly IAssemblyHandler assemblyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerCli"/> class
        /// with specified instances of <see cref="IRunner"/>, <see cref="TextWriter"/>, <see cref="TextReader"/>
        /// and <see cref="IAssemblyHandler"/>.
        /// </summary>
        /// <param name="runner">Specified instance of the <see cref="IRunner"/>.</param>
        /// <param name="textWriter">Specified instance of the <see cref="TextWriter"/>.</param>
        /// <param name="textReader">Specified instance of the <see cref="TextReader"/>.</param>
        /// <param name="assemblyHandler">Specified instance of the <see cref="IAssemblyHandler"/>.</param>
        public RunnerCli(IRunner runner, TextWriter textWriter, TextReader textReader, IAssemblyHandler assemblyHandler)
        {
            this.runner = runner;
            this.textWriter = textWriter;
            this.textReader = textReader;
            this.assemblyHandler = assemblyHandler;
        }

        /// <summary>
        /// Launches CLI.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
                        if (failedTestMethod.ThrownException == null)
                        {
                            await textWriter.WriteLineAsync(
                                $"{failedTestMethod.Name} {failedTestMethod.ElapsedTime}:" +
                                $" expected {failedTestMethod.ExpectedExceptionType}," +
                                " but no exceptions were thrown.");
                        }
                        else
                        {
                            await textWriter.WriteLineAsync(
                                $"{failedTestMethod.Name} {failedTestMethod.ElapsedTime}:" +
                                $" expected {failedTestMethod.ExpectedExceptionType}," +
                                $" but was {failedTestMethod.ThrownException.GetType()}");

                            await textWriter.WriteLineAsync(
                                $"Stack trace: {failedTestMethod.ThrownException.StackTrace}");
                        }
                    }
                    else
                    {
                        await textWriter.WriteLineAsync(
                            $"{failedTestMethod.Name}: {failedTestMethod.ThrownException!.GetType()} was thrown.");

                        await textWriter.WriteLineAsync($"Stack trace: {failedTestMethod.ThrownException.StackTrace}");
                    }

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