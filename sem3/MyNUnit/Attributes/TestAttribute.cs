using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Marks the method as callable from the MyNUnit test runner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TestAttribute : Attribute
    {
        private Type? expected;

        /// <summary>
        /// Expected type of exception.
        /// </summary>
        /// <exception cref="ArgumentException">The class type does not inherit from the <see cref="Exception"/> type.</exception>
        public Type? Expected
        {
            get => expected;
            set
            {
                if (value == null || value.IsSubclassOf(typeof(Exception)))
                {
                    expected = value;
                }
                else
                {
                    throw new ArgumentException(nameof(value));
                }
            }
        }

        /// <summary>
        /// Reason for canceling the test run.
        /// </summary>
        public string? Ignore { get; set; }
    }
}
