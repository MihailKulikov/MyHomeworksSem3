using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    public class IgnoredTestMethod : ITestMethod
    {
        public IgnoredTestMethod(string name, string reasonForIgnoring)
        {
            Name = name;
            ReasonForIgnoring = reasonForIgnoring;
        }

        public string Name { get; }
        public string ReasonForIgnoring { get; }
    }
}