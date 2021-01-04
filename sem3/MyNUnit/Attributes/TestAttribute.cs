using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Marks the method as callable from the MyNUnit test runner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Expected type of exception.
        /// </summary>
        /// <exception cref="ArgumentException">The class type does not inherit from the <see cref="Exception"/> type.</exception>
        public Type? Expected { get; set; }

        /// <summary>
        /// Reason for canceling the test run.
        /// </summary>
        public string? Ignore { get; set; }
    }
}
