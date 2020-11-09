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
                    MyNUnitHandler currentHandler = beforeClassHandler;
                    for (var i = 0; i < testClass.TestMethodInfos.Count; i++)
                    {
                        var afterHandler = new AfterHandler();
                        var testRunHandler = new TestHandler();
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
    }
}
