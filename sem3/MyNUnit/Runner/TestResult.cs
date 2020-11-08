using System;
using System.Collections.Concurrent;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner
{
    public class TestResult
    {
        public TestResult(string className, ConcurrentQueue<Exception> exceptions,
            ConcurrentQueue<ITestMethod> testMethods)
        {
            ClassName = className;
            Exceptions = exceptions;
            TestMethods = testMethods;
        }

        public string ClassName { get; }
        public ConcurrentQueue<Exception> Exceptions { get; }
        public ConcurrentQueue<ITestMethod> TestMethods { get; }
    }
}