using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Runner.Interfaces;

namespace MyNUnit.Runner
{
    /// <summary>
    /// Represents implementation of the <see cref="IAssemblyHandler"/> interface.
    /// </summary>
    public class AssemblyHandler : IAssemblyHandler
    {
        private static IEnumerable<Assembly> GetAssemblies(string pathToAssemblies)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.FullName);
            return Directory.EnumerateFiles(pathToAssemblies, "*.dll").AsParallel()
                .Where(fileName => !currentAssemblies.Contains(fileName)).Select(Assembly.Load);
        }

        public IEnumerable<TestClassWrapper> GetTestClassesFromAssemblies(string pathToAssemblies)
            => GetAssemblies(pathToAssemblies).AsParallel().SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.IsClass)
                .Where(classType => classType.GetMethods().Any(info => info.GetCustomAttributes<TestAttribute>().Any()))
                .Select(classType => new TestClassWrapper(classType));
    }
}