using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner
{
    public class AssemblyHandler : IAssemblyHandler
    {
        public IEnumerable<TestClassWrapper> TestClasses { get; }

        public AssemblyHandler(string pathToAssemblies)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.FullName);
            var assemblies = Directory.EnumerateFiles(pathToAssemblies, "*.dll").AsParallel()
                .Where(fileName => !currentAssemblies.Contains(fileName)).Select(Assembly.Load).ToList();
            TestClasses = GetTestClassesFromAssemblies(assemblies);
        }

        private static IEnumerable<TestClassWrapper> GetTestClassesFromAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies.AsParallel().SelectMany(assembly => assembly.ExportedTypes).Where(type => type.IsClass)
                .Where(classType => classType.GetMethods().Any(info => info.GetCustomAttributes<TestAttribute>().Any()))
                .Select(classType => new TestClassWrapper(classType));
    }
}