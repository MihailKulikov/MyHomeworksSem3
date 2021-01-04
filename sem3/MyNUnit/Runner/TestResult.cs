using System;
using System.Collections.Concurrent;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents class that contains useful information about the tests being run.
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="TestResult"/> class with specified useful information.
        /// </summary>
        /// <param name="className">Name of the test class whose information about running tests this instance store.</param>
        /// <param name="exceptions">Exceptions thrown while running non-test methods.</param>
        /// <param name="testMethods">Collection of the <see cref="ITestMethod"/> instances for storing test runs information.</param>
        public TestResult(string className, ConcurrentQueue<Exception> exceptions,
            ConcurrentQueue<ITestMethod> testMethods)
        {
            ClassName = className;
            Exceptions = exceptions;
            TestMethods = testMethods;
        }

        /// <summary>
        /// Gets name of the test class whose information about running tests this instance store.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Gets exceptions thrown while running non-test methods.
        /// </summary>
        public ConcurrentQueue<Exception> Exceptions { get; }

        /// <summary>
        /// Gets collection of the <see cref="ITestMethod"/> instances for storing test runs information.
        /// </summary>
        public ConcurrentQueue<ITestMethod> TestMethods { get; }
    }
}