using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moq;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;
using NUnit.Framework;

namespace RunnerTests
{
    public class RunnerShould
    {
        private IRunner runner;
        private Mock<ITestClassWrapper> testClassMock;
        private static volatile int afterHandleCallsNumber;
        private static volatile int beforeHandleCallsNumber;
        private static volatile int testHandleCallsNumber;
        private static volatile int afterClassHandleCallsNumber;
        private static volatile int beforeClassHandleCallsNumber;

        public static void BeforeClass()
        {
            Interlocked.Increment(ref beforeClassHandleCallsNumber);
        }

        public static void AfterClass()
        {
            Interlocked.Increment(ref afterClassHandleCallsNumber);
        }
        
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
            runner = new Runner();
            testClassMock = new Mock<ITestClassWrapper>();
            afterHandleCallsNumber = 0;
            beforeHandleCallsNumber = 0;
            testHandleCallsNumber = 0;
            afterClassHandleCallsNumber = 0;
            beforeClassHandleCallsNumber = 0;
        }

        [Test]
        public void Run_Tests()
        {
            testClassMock.Setup(testClass => testClass.ClassType).Returns(GetType()).Verifiable();
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
            testClassMock.Setup(testClass => testClass.AfterClassMethodInfos)
                .Returns(new[]
                {
                    GetType().GetMethod(nameof(AfterClass)),
                    GetType().GetMethod(nameof(AfterClass))
                }).Verifiable();
            testClassMock.Setup(testClass => testClass.BeforeClassMethodInfos)
                .Returns(new[]
                {
                    GetType().GetMethod(nameof(BeforeClass)),
                    GetType().GetMethod(nameof(BeforeClass))
                }).Verifiable();

            var results = runner.RunTests(new[] {testClassMock.Object}).ToList();
            
            testClassMock.Verify();
            testClassMock.VerifyNoOtherCalls();
            foreach (var result in results)
            {
                Assert.That(result.Exceptions, Is.Empty);
                foreach (var testMethod in result.TestMethods)
                {
                    Assert.That(testMethod, Is.TypeOf<SuccessfulTestMethod>());
                    Assert.That(testMethod.Name, Is.EqualTo(nameof(Test)));
                    Assert.That(((SuccessfulTestMethod) testMethod).ElapsedTime, Is.GreaterThan(TimeSpan.Zero));
                }
            }

            Assert.That(afterHandleCallsNumber, Is.EqualTo(6));
            Assert.That(beforeHandleCallsNumber, Is.EqualTo(6));
            Assert.That(testHandleCallsNumber, Is.EqualTo(3));
            Assert.That(afterClassHandleCallsNumber, Is.EqualTo(2));
            Assert.That(beforeClassHandleCallsNumber, Is.EqualTo(2));
        }
    }
}