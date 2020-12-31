using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyNUnit.Attributes;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents test class in MyNUnit.
    /// </summary>
    public class TestClassWrapper : ITestClassWrapper
    {
        private readonly ConcurrentQueue<object?> testClassInstances;

        /// <summary>
        /// Initialize a new instance of the <see cref="TestClassWrapper"/> class which is a wrapper for a test class of the specified type. 
        /// </summary>
        /// <param name="classType">Specified class type.</param>
        public TestClassWrapper(Type classType)
        {
            ClassType = classType;
            BeforeClassMethodInfos = classType.GetMethods().Where(info =>
                info.GetCustomAttributes<BeforeClassAttribute>().Any() && info.IsStatic);
            AfterClassMethodInfos = classType.GetMethods().Where(info =>
                info.GetCustomAttributes<AfterClassAttribute>().Any() && info.IsStatic);
            BeforeMethodInfos = classType.GetMethods()
                .Where(info => info.GetCustomAttributes<BeforeAttribute>().Any());
            AfterMethodInfos = classType.GetMethods()
                .Where(info => info.GetCustomAttributes<AfterAttribute>().Any());
            TestMethodInfos = new ConcurrentQueue<MethodInfo>(classType.GetMethods()
                .Where(info => info.GetCustomAttributes<TestAttribute>().Any())
                .Where(info => info.GetParameters().Length == 0)
                .Where(info => info.ReturnType == typeof(void))
                .Where(info => !info.IsStatic));

            testClassInstances = new ConcurrentQueue<object?>();
            Parallel.For(0, TestMethodInfos.Count,
                num => { testClassInstances.Enqueue(Activator.CreateInstance(classType)); });
        }

        /// <summary>
        /// Gets test class instance.
        /// </summary>
        public object? TestClassInstance =>
            testClassInstances.TryDequeue(out var testClassInstance) ? testClassInstance : null;

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