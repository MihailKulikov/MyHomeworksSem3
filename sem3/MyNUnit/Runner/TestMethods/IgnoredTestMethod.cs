using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    /// <summary>
    /// Represents ignored test method in MyNUnit.
    /// </summary>
    public class IgnoredTestMethod : ITestMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoredTestMethod"/> class with the specified useful information.
        /// </summary>
        /// <param name="name">Name of the test method.</param>
        /// <param name="reasonForIgnoring">Reason for ignoring this test.</param>
        public IgnoredTestMethod(string name, string reasonForIgnoring)
        {
            Name = name;
            ReasonForIgnoring = reasonForIgnoring;
        }

        public string Name { get; }

        /// <summary>
        /// Gets reason for ignoring this test.
        /// </summary>
        public string ReasonForIgnoring { get; }
    }
}