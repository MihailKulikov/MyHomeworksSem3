using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyNUnit.Attributes;

namespace MyNUnit.Runner
{
    public class TestClassWrapper
    {
        private readonly Lazy<object?> testClassLazyInstance;

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

        public object? TestClassInstance => testClassLazyInstance.Value;
        public Type ClassType { get; }
        public IEnumerable<MethodInfo> BeforeClassMethodInfos { get; }
        public IEnumerable<MethodInfo> AfterClassMethodInfos { get; }
        public IEnumerable<MethodInfo> BeforeMethodInfos { get; }
        public IEnumerable<MethodInfo> AfterMethodInfos { get; }
        public ConcurrentQueue<MethodInfo> TestMethodInfos { get; }
    }
}