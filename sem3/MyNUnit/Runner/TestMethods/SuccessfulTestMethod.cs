using System;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    public class SuccessfulTestMethod : ITestMethod
    {
        public SuccessfulTestMethod(string name, TimeSpan elapsedTime)
        {
            Name = name;
            ElapsedTime = elapsedTime;
        }

        public string Name { get; }
        public TimeSpan ElapsedTime { get; }
    }
}