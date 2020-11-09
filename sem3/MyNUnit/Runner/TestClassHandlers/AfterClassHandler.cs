using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling methods with after class attribute.
    /// </summary>
    public class AfterClassHandler : MyNUnitHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterClassHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="nextHandlerIfHandlingWasSuccessful">A handler that will be called upon successful processing of this handler.</param>
        /// <param name="nextHandlerIfHandlingFailed">A handler that will be called upon unsuccessful processing of this handler.</param>
        public AfterClassHandler(MyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            MyNUnitHandler? nextHandlerIfHandlingFailed = null) : base(nextHandlerIfHandlingWasSuccessful,
            nextHandlerIfHandlingFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.AfterClassMethodInfos,
                    afterClassMethod => afterClassMethod.Invoke(null, null));
            }
            catch (Exception e)
            {
                testResult.Exceptions.Enqueue(e);
                return false;
            }

            return true;
        }
    }
}