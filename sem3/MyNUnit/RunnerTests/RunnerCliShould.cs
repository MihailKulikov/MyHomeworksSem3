using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;
using NUnit.Framework;

namespace RunnerTests
{
    public class RunnerCliShould
    {
        private RunnerCli cli;
        private Mock<IRunner> runnerMock;
        private Mock<TextWriter> textWriterMock;
        private Mock<TextReader> textReaderMock;
        private Mock<IAssemblyHandler> assemblyHandlerMock;
        private const string DirectoryNotFoundMessage = "Directory not found.";
        private const string IntroduceMessage = "Enter the path to the assemblies:";
        private const string Path = "Something";
        private const string TestClassName = "Name";

        private const string ExceptionMessage =
            "The following exceptions were thrown in the After, AfterClass, Before, BeforeClass blocks:";

        private static IEnumerable<(ITestMethod testMethod, string expectedMessage)> TestMethodCases
        {
            get
            {
                var ignoredTestMethod = new IgnoredTestMethod("MyName", "Reason");
                var failedTestMethod = new FailedTestMethod("MyName", new Exception(), null, TimeSpan.MaxValue);
                var successfulTestMethod = new SuccessfulTestMethod("MyName", TimeSpan.MaxValue);

                yield return (ignoredTestMethod,
                    $"\t{ignoredTestMethod.Name} was ignored because of {ignoredTestMethod.ReasonForIgnoring}");
                yield return (failedTestMethod,
                    $"\t{failedTestMethod.Name}: {failedTestMethod.ThrownException!.GetType()} was thrown.\n"
                    + $"\tStack trace: {failedTestMethod.ThrownException.StackTrace}");
                yield return (successfulTestMethod,
                    $"\t{successfulTestMethod.Name} passed in {successfulTestMethod.ElapsedTime}");
            }
        }

        private void SimpleVerify()
        {
            textWriterMock.VerifyNoOtherCalls();
            textReaderMock.Verify();
            textReaderMock.VerifyNoOtherCalls();
            assemblyHandlerMock.Verify();
            assemblyHandlerMock.VerifyNoOtherCalls();
            runnerMock.Verify();
            runnerMock.VerifyNoOtherCalls();
        }
        
        [SetUp]
        public void SetUp()
        {
            runnerMock = new Mock<IRunner>();
            textWriterMock = new Mock<TextWriter>();
            textReaderMock = new Mock<TextReader>();
            assemblyHandlerMock = new Mock<IAssemblyHandler>();
            cli = new RunnerCli(runnerMock.Object, textWriterMock.Object, textReaderMock.Object,
                assemblyHandlerMock.Object);
        }

        [Test]
        public void Write_That_Directory_Not_Found_Then_AssemblyHandler_Throw_DirectoryNotFoundException()
        {
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync(Path).Verifiable();
            assemblyHandlerMock.Setup(handler => handler.GetTestClassesFromAssemblies(Path))
                .Throws<DirectoryNotFoundException>().Verifiable();

            cli.Run();

            textWriterMock.Verify(writer => writer.WriteLineAsync(DirectoryNotFoundMessage), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(IntroduceMessage), Times.Once);
            SimpleVerify();
        }

        [Test]
        public void Write_About_Exceptions_Thrown_During_Test_Execution()
        {
            var testClassWrappers = new[] {It.IsAny<ITestClassWrapper>()};
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync(Path).Verifiable();
            assemblyHandlerMock.Setup(handler => handler.GetTestClassesFromAssemblies(Path))
                .Returns(testClassWrappers).Verifiable();
            runnerMock.Setup(runner => runner.RunTests(testClassWrappers)).Returns(new[]
            {
                new TestResult(TestClassName, new ConcurrentQueue<Exception>(new[] {new Exception()}),
                    new ConcurrentQueue<ITestMethod>())
            }).Verifiable();

            cli.Run();

            textWriterMock.Verify(writer => writer.WriteLineAsync(TestClassName), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(IntroduceMessage), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync('\t' + ExceptionMessage), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync('\t' + new Exception().ToString()), Times.Once);
            SimpleVerify();
        }

        [TestCaseSource(nameof(TestMethodCases))]
        [Test]
        public void Write_Info_About_Test_Method((ITestMethod testMethod, string expectedMessage) testMethodCase)
        {
            var (testMethod, expectedMessage) = testMethodCase;
            var testClassWrappers = new[] {It.IsAny<ITestClassWrapper>()};
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync(Path).Verifiable();
            assemblyHandlerMock.Setup(handler => handler.GetTestClassesFromAssemblies(Path)).Returns(testClassWrappers)
                .Verifiable();
            runnerMock.Setup(runner => runner.RunTests(testClassWrappers)).Returns(new[]
            {
                new TestResult(TestClassName, new ConcurrentQueue<Exception>(),
                    new ConcurrentQueue<ITestMethod>(new[] {testMethod}))
            }).Verifiable();

            cli.Run();

            textWriterMock.Verify(writer => writer.WriteLineAsync(TestClassName), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(IntroduceMessage), Times.Once);
            textWriterMock.Verify(
                writer => writer.WriteLineAsync(expectedMessage),
                Times.Once);
            SimpleVerify();
        }
    }
}