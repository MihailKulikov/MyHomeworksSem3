using System;
using System.Threading.Tasks;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling methods with before class attribute.
    /// </summary>
    public class BeforeClassHandler : MyNUnitHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeClassHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public BeforeClassHandler(IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null) : base(nextHandlerIfHandlingWasSuccessful,
            nextHandlerIfHandlingFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, ITestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.BeforeClassMethodInfos,
                    beforeClassMethod => beforeClassMethod.Invoke(null, null));
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