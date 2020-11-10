using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;
using MyNUnit.Runner.TestMethods;
using NUnit.Framework;

namespace RunnerTests.Handlers
{
    public class MultipleBeforeTestAfterHandlerShould
    {
        private IMyNUnitHandler handler;
        private Mock<IMyNUnitHandler> successHandlerMock;
        private Mock<IMyNUnitHandler> failHandlerMock;
        private Mock<ITestClassWrapper> testClassMock;
        private TestResult testResult;
        private volatile int afterHandleCallsNumber;
        private volatile int beforeHandleCallsNumber;
        private volatile int testHandleCallsNumber;

        public void Before()
        {
            Interlocked.Increment(ref beforeHandleCallsNumber);
        }

        public void After()
        {
            Interlocked.Increment(ref afterHandleCallsNumber);
        }

        [MyNUnit.Attributes.Test]
        public void Test()
        {
            Interlocked.Increment(ref testHandleCallsNumber);
        }

        [SetUp]
        public void SetUp()
        {
            testResult = new TestResult("Something", new ConcurrentQueue<Exception>(),
                new ConcurrentQueue<ITestMethod>());
            testClassMock = new Mock<ITestClassWrapper>();
            failHandlerMock = new Mock<IMyNUnitHandler>();
            successHandlerMock = new Mock<IMyNUnitHandler>();
            handler = new MultipleBeforeTestAfterHandler(successHandlerMock.Object, failHandlerMock.Object);
            afterHandleCallsNumber = 0;
            beforeHandleCallsNumber = 0;
            testHandleCallsNumber = 0;
        }
        
        [Test]
        public void Call_HandleMethods_Of_Before_Test_After_Handlers()
        {
            testClassMock.Setup(testClass => testClass.TestClassInstance).Returns(this).Verifiable();
            testClassMock.Setup(testClass => testClass.TestMethodInfos).Returns(
                new ConcurrentQueue<MethodInfo>(new[]
                {
                    GetType().GetMethod(nameof(Test)),
                    GetType().GetMethod(nameof(Test)),
                    GetType().GetMethod(nameof(Test))
                })).Verifiable();
            testClassMock.Setup(testClass => testClass.AfterMethodInfos)
                .Returns(new[]
                {
                    GetType().GetMethod(nameof(After)),
                    GetType().GetMethod(nameof(After))
                }).Verifiable();
            testClassMock.Setup(testClass => testClass.BeforeMethodInfos)
                .Returns(new[]
                {
                    GetType().GetMethod(nameof(Before)),
                    GetType().GetMethod(nameof(Before))
                }).Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            Assert.That(testResult.Exceptions, Is.Empty);
            Assert.That(testResult.TestMethods.Count, Is.EqualTo(3));
            foreach (var testMethod in testResult.TestMethods)
            {
                Assert.That(testMethod.Name, Is.EqualTo(nameof(Test)));
                Assert.That(testMethod, Is.TypeOf<SuccessfulTestMethod>());
                Assert.That(((SuccessfulTestMethod) testMethod).ElapsedTime, Is.GreaterThan(TimeSpan.Zero));
            }

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            
            successHandlerMock.Verify(successHandler => successHandler.Handle(testResult, testClassMock.Object),
                Times.Once);
            successHandlerMock.VerifyNoOtherCalls();
            
            failHandlerMock.VerifyNoOtherCalls();

            Assert.That(testHandleCallsNumber, Is.EqualTo(3));
            Assert.That(afterHandleCallsNumber, Is.EqualTo(6));
            Assert.That(beforeHandleCallsNumber, Is.EqualTo(6));
        }
    }
}