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
        private static IEnumerable<Assembly> GetAssemblies(string pathToAssemblies) =>
            Directory.EnumerateFiles(pathToAssemblies, "*.dll")
                .Select(addInAssembly =>
                {
                    try
                    {
                        return Assembly.LoadFrom(addInAssembly!);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .Where(addInAssembly => addInAssembly != null)
                .Select(addInAssembly => addInAssembly!);

        public IEnumerable<ITestClassWrapper> GetTestClassesFromAssemblies(string pathToAssemblies) =>
            GetAssemblies(pathToAssemblies).SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.IsClass)
                .Where(classType => classType.GetMethods().Any(info => info.GetCustomAttributes<TestAttribute>().Any()))
                .Select(classType => new TestClassWrapper(classType));
    }
}