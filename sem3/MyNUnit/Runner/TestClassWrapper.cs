using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyNUnit.Attributes;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents test class in MyNUnit.
    /// </summary>
    public class TestClassWrapper
    {
        private readonly Lazy<object?> testClassLazyInstance;

        /// <summary>
        /// Initialize a new instance of the <see cref="TestClassWrapper"/> class which is a wrapper for a test class of the specified type. 
        /// </summary>
        /// <param name="classType">Specified class type.</param>
        public TestClassWrapper(Type classType)
        {
            ClassType = classType;
            testClassLazyInstance =
                new Lazy<object?>(() => Activator.CreateInstance(ClassType), true);
            BeforeClassMethodInfos = classType.GetMethods().Where(info =>
                info.GetCustomAttributes<BeforeClassAttribute>().Any() && info.IsStatic);
            AfterClassMethodInfos = classType.GetMethods().Where(info =>
                info.GetCustomAttributes<AfterClassAttribute>().Any() && info.IsStatic);
            BeforeMethodInfos = classType.GetMethods()
                .Where(info => info.GetCustomAttributes<BeforeAttribute>().Any());
            AfterMethodInfos = classType.GetMethods()
                .Where(info => info.GetCustomAttributes<AfterAttribute>().Any());
            TestMethodInfos = new ConcurrentQueue<MethodInfo>(classType.GetMethods()
                .Where(info => info.GetCustomAttributes<TestAttribute>().Any()));
        }

        /// <summary>
        /// Gets test class instance.
        /// </summary>
        public object? TestClassInstance => testClassLazyInstance.Value;

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