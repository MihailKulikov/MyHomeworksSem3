using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;

namespace MyNUnit.Runner
{
    public class Runner : IRunner
    {
        public IEnumerable<TestResult> RunTests(IEnumerable<TestClassWrapper> testClasses) =>
            testClasses.AsParallel().Select(testClass =>
                {
                    var afterClassHandler = new AfterClassHandler();
                    var beforeClassHandler = new BeforeClassHandler(nextHandlerIfHandleFailed: afterClassHandler);
                    TestClassHandler currentHandler = beforeClassHandler;
                    for (var i = 0; i < testClass.TestMethodInfos.Count; i++)
                    {
                        var afterHandler = new AfterHandler();
                        var testRunHandler = new TestRunHandler();
                        currentHandler.NextHandlerIfHandleSuccess =
                            new BeforeHandler(testRunHandler, afterHandler);
                        testRunHandler.NextHandlerIfHandleSuccess = afterHandler;
                        currentHandler = afterHandler;
                    }

                    currentHandler.NextHandlerIfHandleFailed = afterClassHandler;
                    currentHandler.NextHandlerIfHandleSuccess = afterClassHandler;

                    return beforeClassHandler.Handle(new TestResult(testClass.ClassType.FullName ?? "",
                        new ConcurrentQueue<Exception>(), new ConcurrentQueue<ITestMethod>()), testClass);
                }
            );

        // public IEnumerable<ITestResults> RunTests()
        // {
        //     return testClasses.AsParallel().Select<TestClassWrapper, ITestResults>(testClass =>
        //     {
        //         try
        //         {
        //             Parallel.ForEach(testClass.BeforeClassMethodInfos,
        //                 beforeClassMethod => beforeClassMethod.Invoke(null, null));
        //         }
        //         catch (Exception e)
        //         {
        //             return new FailedTestResults(testClass.ClassType.FullName ?? "", e);
        //         }
        //         
        //         var instanceOfTestClass = Activator.CreateInstance(testClass.ClassType);
        //
        //         var testMethods = testClass.TestMethodInfos.AsParallel().Select(testMethod =>
        //         {
        //             TestAttribute testAttribute = testMethod.GetCustomAttribute<TestAttribute>()!;
        //             if (testAttribute.Ignore != null)
        //             {
        //                 return new IgnoredTestMethod(testMethod.Name, testAttribute.Ignore);
        //             }
        //             
        //             var stopwatch = new Stopwatch();
        //             stopwatch.Start();
        //             try
        //             {
        //                 Parallel.ForEach(testClass.BeforeMethodInfos,
        //                     beforeClassMethod => beforeClassMethod.Invoke(instanceOfTestClass, null));
        //             }
        //             catch (Exception e)
        //             {
        //                 stopwatch.Stop();
        //                 return new FailedTestMethod(testMethod.Name, e, testAttribute.Expected, stopwatch.Elapsed);
        //             }
        //
        //             try
        //             {
        //                 testMethod.Invoke(instanceOfTestClass, null);
        //                 try
        //                 {
        //                     Parallel.ForEach(testClass.AfterMethodInfos,
        //                         afterMethod => afterMethod.Invoke(instanceOfTestClass, null));
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     stopwatch.Stop();
        //                     return new FailedTestMethod(testMethod.Name, e, null, stopwatch.Elapsed);
        //                 }
        //             }
        //             catch (Exception e)
        //             {
        //                 stopwatch.Stop();
        //                 if (e.GetType() == testAttribute.Expected)
        //                 {
        //                     return new SuccessfulTestMethod(testMethod.Name, stopwatch.Elapsed);
        //                 }
        //                 
        //                 return new FailedTestMethod(testMethod.Name, e, testAttribute.Expected, stopwatch.Elapsed);
        //             }
        //             stopwatch.Stop();
        //             
        //             return new SuccessfulTestMethod(testMethod.Name, stopwatch.Elapsed);
        //         });
        //     });
    }
}
