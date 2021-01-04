using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents base handler in chain of responsibility pattern.
    /// </summary>
    public abstract class MyNUnitHandler : IMyNUnitHandler
    {
        /// <summary>
        /// Gets and sets value of the next handler in chain if handling was successful.
        /// </summary>
        public IMyNUnitHandler? NextHandlerIfHandlingWasSuccessful { get; set; }

        /// <summary>
        /// Gets and sets value of the next handler in chain if handling failed.
        /// </summary>
        public IMyNUnitHandler? NextHandlerIfHandlingFailed { get; set; }

        protected MyNUnitHandler(IMyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            IMyNUnitHandler? nextHandlerIfHandlingFailed = null)
        {
            NextHandlerIfHandlingWasSuccessful = nextHandlerIfHandlingWasSuccessful;
            NextHandlerIfHandlingFailed = nextHandlerIfHandlingFailed;
        }

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="testResult">Contains useful information about the tests being run.</param>
        /// <param name="testClass">Contains test, before, after, beforeClass, afterClass methods being run in this launch.</param>
        /// <returns>Contains updated information about the tests being run.</returns>
        public TestResult Handle(TestResult testResult, ITestClassWrapper testClass) =>
            RunMethods(testResult, testClass)
                ? NextHandlerIfHandlingWasSuccessful?.Handle(testResult, testClass) ?? testResult
                : NextHandlerIfHandlingFailed?.Handle(testResult, testClass) ?? testResult;

        /// <summary>
        /// Determines the success of the handler processing.
        /// </summary>
        /// <param name="testResult">Contains useful information about the tests being run.</param>
        /// <param name="testClass">Contains test, before, after, beforeClass, afterClass methods being run in this launch.</param>
        /// <returns>true if processing was successful; otherwise false.</returns>
        protected abstract bool RunMethods(TestResult testResult, ITestClassWrapper testClass);
    }
}