using System;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestMethods;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling methods with test attribute.
    /// </summary>
    public class TestHandler : MyNUnitHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public TestHandler(IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null)
            : base(nextHandlerIfHandlingWasSuccessful, nextHandlerIfHandlingFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, ITestClassWrapper testClass)
        {
            if (!testClass.TestMethodInfos.TryDequeue(out var testMethod)) return true;
            var attribute = testMethod.GetCustomAttribute<TestAttribute>()!;
            if (attribute.Ignore != null)
            {
                testResult.TestMethods
                    .Enqueue(new IgnoredTestMethod(testMethod.Name, attribute.Ignore));

                return true;
            }

            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                testMethod.Invoke(testClass.TestClassInstance, null);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                if (e.GetType() == attribute.Expected)
                {
                    testResult.TestMethods.Enqueue(new SuccessfulTestMethod(testMethod.Name, stopwatch.Elapsed));
                    return true;
                }

                testResult.TestMethods.Enqueue(new FailedTestMethod(testMethod.Name, e, attribute.Expected,
                    stopwatch.Elapsed));
                return true;
            }

            stopwatch.Stop();
            if (attribute.Expected != null)
            {
                testResult.TestMethods.Enqueue(new FailedTestMethod(testMethod.Name, null, attribute.Expected,
                    stopwatch.Elapsed));
            }
            else
            {
                testResult.TestMethods.Enqueue(new SuccessfulTestMethod(testMethod.Name, stopwatch.Elapsed));
            }

            return true;
        }
    }
}