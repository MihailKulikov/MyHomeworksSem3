using System;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling methods with after attribute.
    /// </summary>
    public class AfterHandler : MyNUnitHandler
    {
        private readonly object? testClassInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="testClassInstance">Test class instance to which the method will be applied.</param>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public AfterHandler(object? testClassInstance, IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null) :
            base(nextHandlerIfHandlingWasSuccessful, nextHandlerIfHandlingFailed)
        {
            this.testClassInstance = testClassInstance;
        }

        protected override bool RunMethods(TestResult testResult, ITestClassWrapper testClass)
        {
            try
            {
                foreach (var afterMethod in testClass.AfterMethodInfos)
                {
                    afterMethod.Invoke(testClassInstance, null);
                }
            }
            catch (Exception e)
            {
                testResult.Exceptions.Enqueue((e.InnerException?.InnerException ?? e.InnerException) ?? e);
                return false;
            }

            return true;
        }
    }
}