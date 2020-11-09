using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MyNUnit.Runner.Interfaces;
using MyNUnit.Runner.TestClassHandlers;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents implementation of the <see cref="IRunner"/> interface.
    /// </summary>
    public class Runner : IRunner
    {
        public IEnumerable<TestResult> RunTests(IEnumerable<TestClassWrapper> testClasses) =>
            testClasses.AsParallel().Select(testClass =>
                {
                    var afterClassHandler = new AfterClassHandler();
                    var beforeClassHandler = new BeforeClassHandler(nextHandlerIfHandlingFailed: afterClassHandler);
                    var multipleTestHandler = new MultipleBeforeTestAfterHandler(afterClassHandler);
                    beforeClassHandler.NextHandlerIfHandlingWasSuccessful = multipleTestHandler;

                    return beforeClassHandler.Handle(new TestResult(testClass.ClassType.FullName ?? "",
                        new ConcurrentQueue<Exception>(), new ConcurrentQueue<ITestMethod>()), testClass);
                }
            );
    }
}
