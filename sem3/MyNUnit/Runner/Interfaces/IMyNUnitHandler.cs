namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Represents handler in chain of responsibility pattern.
    /// </summary>
    public interface IMyNUnitHandler
    {
        /// <summary>
        /// Gets and sets value of the next handler in chain if handling was successful.
        /// </summary>
        IMyNUnitHandler? NextHandlerIfHandlingWasSuccessful { get; set; }
        
        /// <summary>
        /// Gets and sets value of the next handler in chain if handling failed.
        /// </summary>
        IMyNUnitHandler? NextHandlerIfHandlingFailed { get; set; }
        
        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="testResult">Contains useful information about the tests being run.</param>
        /// <param name="testClass">Contains test, before, after, beforeClass, afterClass methods being run in this launch.</param>
        /// <returns>Contains updated information about the tests being run.</returns>
        TestResult Handle(TestResult testResult, ITestClassWrapper testClass);
    }
}