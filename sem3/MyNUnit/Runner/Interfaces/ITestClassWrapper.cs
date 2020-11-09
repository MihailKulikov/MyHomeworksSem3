using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Represents test class in MyNUnit.
    /// </summary>
    public interface ITestClassWrapper
    {
        /// <summary>
        /// Gets test class instance.
        /// </summary>
        public object? TestClassInstance { get; }

        /// <summary>
        /// Gets test class type.
        /// </summary>
        public Type ClassType { get; }

        /// <summary>
        /// Gets collection of the method infos with before class attribute.
        /// </summary>
        public IEnumerable<MethodInfo> BeforeClassMethodInfos { get; }

        /// <summary>
        /// Gets collection of the method infos with after class attribute.
        /// </summary>
        public IEnumerable<MethodInfo> AfterClassMethodInfos { get; }

        /// <summary>
        /// Gets collection of the method infos with before attribute.
        /// </summary>
        public IEnumerable<MethodInfo> BeforeMethodInfos { get; }

        /// <summary>
        /// Gets collection of the method infos with after attribute.
        /// </summary>
        public IEnumerable<MethodInfo> AfterMethodInfos { get; }

        /// <summary>
        /// Collection of the method infos with test attribute.
        /// </summary>
        public ConcurrentQueue<MethodInfo> TestMethodInfos { get; }
    }
}