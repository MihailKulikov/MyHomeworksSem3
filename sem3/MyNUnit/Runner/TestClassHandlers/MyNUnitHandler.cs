namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents base handler in chain of responsibility pattern.
    /// </summary>
    public abstract class MyNUnitHandler
    {
        /// <summary>
        /// Gets and sets value of the next handler in chain if handling was successful.
        /// </summary>
        public MyNUnitHandler? NextHandlerIfHandlingWasSuccessful { get; set; }

        /// <summary>
        /// Gets and sets value of the next handler in chain if handling failed.
        /// </summary>
        public MyNUnitHandler? NextHandlerIfHandlingFailed { get; set; }

        protected MyNUnitHandler(MyNUnitHandler? nextHandlerIfHandlingWasSuccessful = null,
            MyNUnitHandler? nextHandlerIfHandlingFailed = null)
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
        public TestResult Handle(TestResult testResult, TestClassWrapper testClass)
        {
            return RunMethods(testResult, testClass)
                ? NextHandlerIfHandlingWasSuccessful?.Handle(testResult, testClass) ?? testResult
                : NextHandlerIfHandlingFailed?.Handle(testResult, testClass) ?? testResult;
        }

        /// <summary>
        /// Determines the success of the handler processing.
        /// </summary>
        /// <param name="testResult">Contains useful information about the tests being run.</param>
        /// <param name="testClass">Contains test, before, after, beforeClass, afterClass methods being run in this launch.</param>
        /// <returns>true if processing was successful; otherwise false.</returns>
        protected abstract bool RunMethods(TestResult testResult, TestClassWrapper testClass);
    }
}