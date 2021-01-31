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
        /// <param name="classType">Type of the test class whose information about running tests this instance store.</param>
        /// <param name="exceptions">Exceptions thrown while running non-test methods.</param>
        /// <param name="testMethods">Collection of the <see cref="ITestMethod"/> instances for storing test runs information.</param>
        public TestResult(Type classType, ConcurrentQueue<Exception> exceptions,
            ConcurrentQueue<ITestMethod> testMethods)
        {
            ClassType = classType;
            Exceptions = exceptions;
            TestMethods = testMethods;
        }

        /// <summary>
        /// Gets type of the test class whose information about running tests this instance store.
        /// </summary>
        public Type ClassType { get; }

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