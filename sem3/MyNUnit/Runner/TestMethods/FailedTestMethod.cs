using System;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    /// <summary>
    /// Represents failed test method in MyNUnit.
    /// </summary>
    public class FailedTestMethod : ITestMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedTestMethod"/> with the specified useful information.
        /// </summary>
        /// <param name="name">Name of the test method.</param>
        /// <param name="thrownException">Exception that was thrown.</param>
        /// <param name="expectedExceptionType">Expected type of the thrown exception.</param>
        /// <param name="elapsedTime">Time taken to run the test.</param>
        public FailedTestMethod(string name, Exception? thrownException, Type? expectedExceptionType,
            TimeSpan elapsedTime)
        {
            Name = name;
            ThrownException = thrownException;
            ElapsedTime = elapsedTime;
            ExpectedExceptionType = expectedExceptionType;
        }

        public string Name { get; }

        /// <summary>
        /// Gets exception that was thrown.
        /// </summary>
        public Exception? ThrownException { get; }

        /// <summary>
        /// Gets expected type of the thrown exception.
        /// </summary>
        public Type? ExpectedExceptionType { get; }

        /// <summary>
        /// Gets time taken to run the test.
        /// </summary>
        public TimeSpan ElapsedTime { get; }
    }
}