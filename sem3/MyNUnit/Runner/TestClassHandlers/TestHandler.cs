using System;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Runner.TestMethods;

namespace MyNUnit.Runner.TestClassHandlers
{
    public class TestHandler : MyNUnitHandler
    {
        public TestHandler(MyNUnitHandler? nextHandlerIfHandleSuccess = null,
            MyNUnitHandler? nextHandlerIfHandleFailed = null)
            : base(nextHandlerIfHandleSuccess, nextHandlerIfHandleFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
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
            testResult.TestMethods.Enqueue(new SuccessfulTestMethod(testMethod.Name, stopwatch.Elapsed));
            return true;
        }
    }
}