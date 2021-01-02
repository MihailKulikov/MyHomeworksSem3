using System;
using System.Threading.Tasks;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling multiple blocks of before, test, after methods.
    /// </summary>
    public class MultipleBeforeTestAfterHandler : MyNUnitHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleBeforeTestAfterHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public MultipleBeforeTestAfterHandler(IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null) : base(nextHandlerIfHandlingWasSuccessful,
            nextHandlerIfHandlingFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, ITestClassWrapper testClass)
        {
            var testCount = testClass.TestMethodInfos.Count;
            Parallel.For(0, testCount, index =>
            {
                var testClassInstance = Activator.CreateInstance(testClass.ClassType);
                var testHandler = new TestHandler(testClassInstance);
                var beforeHandler = new BeforeHandler(testClassInstance);
                var afterHandler = new AfterHandler(testClassInstance);
                testHandler.NextHandlerIfHandlingWasSuccessful = afterHandler;
                beforeHandler.NextHandlerIfHandlingWasSuccessful = testHandler;
                beforeHandler.NextHandlerIfHandlingFailed = afterHandler;
                beforeHandler.Handle(testResult, testClass);
            });

            return true;
        }
    }
}