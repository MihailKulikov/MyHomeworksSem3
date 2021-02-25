using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;
using NUnit.Framework;

namespace RunnerTests.Handlers
{
    public class BeforeAndAfterHandlerShould
    {
        private IMyNUnitHandler handler;
        private Mock<IMyNUnitHandler> successHandlerMock;
        private Mock<IMyNUnitHandler> failHandlerMock;
        private Mock<ITestClassWrapper> testClassMock;
        private TestResult testResult;
        private static int count;
        private static readonly Exception Exception = new Exception();
        private static object testClassInstance;

        private static
            IEnumerable<(Func<IMyNUnitHandler, IMyNUnitHandler, IMyNUnitHandler> initializeHandler,
                Expression<Func<ITestClassWrapper, IEnumerable<MethodInfo>>> getMethods)> BeforeOrAfterClassCase
        {
            get
            {
                yield return (
                    (successfulHandler, failHandler) => new BeforeClassHandler(successfulHandler, failHandler),
                    testClass => testClass.BeforeClassMethodInfos);
                yield return ((successfulHandler, failHandler) => new AfterClassHandler(successfulHandler, failHandler),
                    testClass => testClass.AfterClassMethodInfos);
                yield return ((successfulHandler, failHandler) => new BeforeHandler(testClassInstance, successfulHandler, failHandler),
                    testClass => testClass.BeforeMethodInfos);
                yield return ((successfulHandler, failHandler) => new AfterHandler(testClassInstance, successfulHandler, failHandler),
                    testClass => testClass.AfterMethodInfos);
            }
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
            testClassInstance = Activator.CreateInstance(GetType());
            count = 0;
            testResult = new TestResult(It.IsAny<Type>(), new ConcurrentQueue<Exception>(),
                new ConcurrentQueue<ITestMethod>());
            testClassMock = new Mock<ITestClassWrapper>();
            successHandlerMock = new Mock<IMyNUnitHandler>();
            failHandlerMock = new Mock<IMyNUnitHandler>();
        }

        [TestCaseSource(nameof(BeforeOrAfterClassCase))]
        [Test]
        public void Call_Handle_Method_Of_Success_Handler_If_Processing_Was_Success(
            (Func<IMyNUnitHandler, IMyNUnitHandler, IMyNUnitHandler> initializeHandler,
                Expression<Func<ITestClassWrapper, IEnumerable<MethodInfo>>> getMethods) testCase)
        {
            var (initializeHandler, getMethods) = testCase;
            handler = initializeHandler!(successHandlerMock.Object, failHandlerMock.Object);
            testClassMock.Setup(getMethods)
                .Returns(new[] {GetType().GetMethod(nameof(IncrementGlobalParameter))}).Verifiable();

            var actualTestResult = handler.Handle(testResult, testClassMock.Object);

            Assert.That(count, Is.EqualTo(1));
            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            successHandlerMock.Verify(successHandler => successHandler.Handle(testResult, testClassMock.Object),
                Times.Once);
            successHandlerMock.VerifyNoOtherCalls();
            failHandlerMock.VerifyNoOtherCalls();
            Assert.That(JsonSerializer.Serialize(actualTestResult),
                Is.EqualTo(JsonSerializer.Serialize(testResult)));
        }

        [TestCaseSource(nameof(BeforeOrAfterClassCase))]
        [Test]
        public void Call_Handle_Method_Of_Fail_Handler_And_Collect_Exception_If_Handling_Failed(
            (Func<IMyNUnitHandler, IMyNUnitHandler, IMyNUnitHandler> initializeHandler,
                Expression<Func<ITestClassWrapper, IEnumerable<MethodInfo>>> getMethods) testCase)
        {
            var (initializeHandler, getMethods) = testCase;
            handler = initializeHandler(successHandlerMock.Object, failHandlerMock.Object);
            var expectedExceptions = new ConcurrentQueue<Exception>();
            expectedExceptions.Enqueue(Exception);
            testClassMock.Setup(getMethods)
                .Returns(new[] {GetType().GetMethod(nameof(ThrowException))}).Verifiable();

            var actualTestResult = handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            successHandlerMock.VerifyNoOtherCalls();
            failHandlerMock.Verify(failHandler => failHandler.Handle(testResult, testClassMock.Object), Times.Once);
            failHandlerMock.VerifyNoOtherCalls();
            Assert.That(actualTestResult.Exceptions.Count, Is.EqualTo(1));
            actualTestResult.Exceptions.TryDequeue(out var exception);
            Assert.That(exception, Is.EqualTo(Exception));
        }
    }
}