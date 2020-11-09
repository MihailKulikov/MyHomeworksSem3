using System;
using System.Collections.Concurrent;
using System.Reflection;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;
using NUnit.Framework;

namespace RunnerTests.Handlers
{
    public class TestHandlerShould
    {
        private const string ReasonForIgnoring = "Haha";
        private IMyNUnitHandler handler;
        private Mock<IMyNUnitHandler> successHandlerMock;
        private Mock<IMyNUnitHandler> failHandlerMock;
        private Mock<ITestClassWrapper> testClassMock;
        private TestResult testResult;
        private static int count;
        private static readonly Exception Exception = new Exception();

        [MyNUnit.Attributes.Test(Ignore = ReasonForIgnoring)]
        public void IgnoredTest()
        {
        }
        
        public static void IncrementGlobalParameter()
        {
            count++;
        }

        public static void ThrowException()
        {
            throw Exception;
        }
        
        [SetUp]
        public void SetUp()
        {
            count = 0;
            testResult = new TestResult("Something", new ConcurrentQueue<Exception>(),
                new ConcurrentQueue<ITestMethod>());
            testClassMock = new Mock<ITestClassWrapper>();
            successHandlerMock = new Mock<IMyNUnitHandler>();
            failHandlerMock = new Mock<IMyNUnitHandler>();
            handler = new TestHandler(successHandlerMock.Object, failHandlerMock.Object);
        }

        [Test]
        public void Call_HandleMethod_Of_Success_Handler_When_There_Are_No_Tests_In_The_Test_Class()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos).Returns(new ConcurrentQueue<MethodInfo>())
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);
            
            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            successHandlerMock.Verify(unitHandler => unitHandler.Handle(testResult, testClassMock.Object), Times.Once);
            successHandlerMock.VerifyNoOtherCalls();
            failHandlerMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Collect_Ignored_Tests_To_TestResult()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos).Returns();
        }
    }
}