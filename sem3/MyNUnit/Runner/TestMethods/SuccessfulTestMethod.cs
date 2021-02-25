using System;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    /// <summary>
    /// Represents successful test in MyNUnit.
    /// </summary>
    public class SuccessfulTestMethod : ITestMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulTestMethod"/> class with the specified useful information.
        /// </summary>
        /// <param name="name">Name of the test method.</param>
        /// <param name="elapsedTime">Time taken to run the test.</param>
        public SuccessfulTestMethod(string name, TimeSpan elapsedTime)
        {
            Name = name;
            ElapsedTime = elapsedTime;
        }

        public string Name { get; }

        /// <summary>
        /// Gets time taken to run the test.
        /// </summary>
        public TimeSpan ElapsedTime { get; }
    }
}