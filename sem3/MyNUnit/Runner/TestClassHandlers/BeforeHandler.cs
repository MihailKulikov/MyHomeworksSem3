using System;
using System.Threading.Tasks;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling method with before attribute.
    /// </summary>
    public class BeforeHandler : MyNUnitHandler
    {
        private readonly object? testClassInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="testClassInstance">Test class instance to which the method will be applied.</param>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public BeforeHandler(object? testClassInstance, IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null)
            : base(nextHandlerIfHandlingWasSuccessful, nextHandlerIfHandlingFailed)
        {
            this.testClassInstance = testClassInstance;
        }

        protected override bool RunMethods(TestResult testResult, ITestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.BeforeMethodInfos,
                    beforeMethod => beforeMethod.Invoke(testClassInstance, null));
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