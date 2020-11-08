using System;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner.TestMethods
{
    public class FailedTestMethod : ITestMethod
    {
        public FailedTestMethod(string name, Exception thrownException, Type? expectedExceptionType, TimeSpan elapsedTime)
        {
            Name = name;
            ThrownException = thrownException;
            ElapsedTime = elapsedTime;
            ExpectedExceptionType = expectedExceptionType;
        }

        public string Name { get; }
        
        public Exception ThrownException { get; }
        public Type? ExpectedExceptionType { get; }
        public TimeSpan ElapsedTime { get; }
    }
}