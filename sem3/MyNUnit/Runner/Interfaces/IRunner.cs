using System.Collections.Generic;

namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Provides method for launching tests.
    /// </summary>
    public interface IRunner
    {
        /// <summary>
        /// Runs tests in specified collection of the <see cref="TestClassWrapper"/> instances.
        /// </summary>
        /// <param name="testClasses">Specified collection of the <see cref="TestClassWrapper"/> instances.</param>
        /// <returns>Collection of <see cref="TestResult"/> instances
        /// containing all the useful information about the tests being run.</returns>
        IEnumerable<TestResult> RunTests(IEnumerable<TestClassWrapper> testClasses);
    }
}