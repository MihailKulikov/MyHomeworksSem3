using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;
using MyNUnit.Runner.TestMethods;
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
        private int count;
        private static readonly Exception Exception = new Exception();

        [MyNUnit.Attributes.Test(Ignore = ReasonForIgnoring)]
        public void IgnoredTest()
        {
            count++;
        }

        [MyNUnit.Attributes.Test(Expected = typeof(AggregateException))]
        public void ThrowsExpectedException()
        {
            throw new AggregateException();
        }

        [MyNUnit.Attributes.Test(Expected = typeof(ArgumentException))]
        public void DoesNotThrowExpectedException()
        {
        }

        [MyNUnit.Attributes.Test]
        public void ThrowUnexpectedException()
        {
            throw Exception;
        }

        [MyNUnit.Attributes.Test]
        public void GoodTest()
        {
            count++;
        }

        private void VerifyCallingHandleMethodOfNextHandlers()
        {
            successHandlerMock.Verify(unitHandler => unitHandler.Handle(testResult, testClassMock.Object), Times.Once);
            successHandlerMock.VerifyNoOtherCalls();
            failHandlerMock.VerifyNoOtherCalls();
        }

        private void AssertFailureTest(Exception expectedThrownException, Type expectedTypeOfExpectedException,
            string testMethodName)
        {
            Assert.That(testResult.Exceptions, Is.Empty);
            Assert.That(testResult.TestMethods.Count, Is.EqualTo(1));
            var testMethod = testResult.TestMethods.First();
            Assert.That(testMethod, Is.TypeOf<FailedTestMethod>());
            Assert.That(((FailedTestMethod) testMethod).ThrownException, Is.EqualTo(expectedThrownException));
            Assert.That(((FailedTestMethod) testMethod).ExpectedExceptionType,
                Is.EqualTo(expectedTypeOfExpectedException));
            Assert.That(((FailedTestMethod) testMethod).ElapsedTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(testMethod.Name, Is.EqualTo(testMethodName));
        }

        private void AssertSuccessfulTest(string testMethodName)
        {
            Assert.That(testResult.Exceptions, Is.Empty);
            Assert.That(testResult.TestMethods.Count, Is.EqualTo(1));
            var testMethod = testResult.TestMethods.First();
            Assert.That(testMethod, Is.TypeOf<SuccessfulTestMethod>());
            Assert.That(((SuccessfulTestMethod) testMethod).ElapsedTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(testMethod.Name, Is.EqualTo(testMethodName));
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
            handler = new TestHandler(Activator.CreateInstance(GetType()), successHandlerMock.Object,
                failHandlerMock.Object);
        }

        [Test]
        public void Call_HandleMethod_Of_Success_Handler_When_There_Are_No_Tests_In_The_Test_Class()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos).Returns(new ConcurrentQueue<MethodInfo>())
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
        }

        [Test]
        public void Collect_Ignored_Tests_To_TestResult()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos)
                .Returns(new ConcurrentQueue<MethodInfo>(new[] {GetType().GetMethod(nameof(IgnoredTest))}))
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
            Assert.That(testResult.Exceptions, Is.Empty);
            Assert.That(testResult.TestMethods.Count, Is.EqualTo(1));
            var testMethod = testResult.TestMethods.First();
            Assert.That(testMethod, Is.TypeOf<IgnoredTestMethod>());
            Assert.That(((IgnoredTestMethod) testMethod)?.ReasonForIgnoring, Is.EqualTo(ReasonForIgnoring));
            Assert.That(testMethod?.Name, Is.EqualTo(nameof(IgnoredTest)));
            Assert.That(count, Is.Zero);
        }

        [Test]
        public void Collect_Tests_That_Throw_Expected_Exception()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos)
                .Returns(new ConcurrentQueue<MethodInfo>(new[] {GetType().GetMethod(nameof(ThrowsExpectedException))}))
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
            AssertSuccessfulTest(nameof(ThrowsExpectedException));
        }

        [Test]
        public void Collect_Tests_That_Throw_Unexpected_Exception()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos)
                .Returns(new ConcurrentQueue<MethodInfo>(new[] {GetType().GetMethod(nameof(ThrowUnexpectedException))}))
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
            AssertFailureTest(Exception, null, nameof(ThrowUnexpectedException));
        }

        [Test]
        public void Collect_Tests_That_Does_Not_Throw_Expected_Exceptions()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos)
                .Returns(new ConcurrentQueue<MethodInfo>(new[]
                    {GetType().GetMethod(nameof(DoesNotThrowExpectedException))}))
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
            AssertFailureTest(null, typeof(ArgumentException), nameof(DoesNotThrowExpectedException));
        }

        [Test]
        public void Collect_Tests_That_Pass_Successfully()
        {
            testClassMock.Setup(testClass => testClass.TestMethodInfos)
                .Returns(new ConcurrentQueue<MethodInfo>(new[] {GetType().GetMethod(nameof(GoodTest))}))
                .Verifiable();

            handler.Handle(testResult, testClassMock.Object);

            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            VerifyCallingHandleMethodOfNextHandlers();
            AssertSuccessfulTest(nameof(GoodTest));
        }
    }
}