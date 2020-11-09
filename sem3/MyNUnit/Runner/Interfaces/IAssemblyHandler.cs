using System.Collections.Generic;

namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Provides method for getting test classes from assemblies in the specified directory.
    /// </summary>
    public interface IAssemblyHandler
    {
        /// <summary>
        /// Gets test classes from assemblies in the specified directory.
        /// </summary>
        /// <param name="pathToAssemblies">Specified path to the directory that contains assemblies.</param>
        /// <returns>Instances of the <see cref="TestClassWrapper"/> that represent test classes.</returns>
        IEnumerable<TestClassWrapper> GetTestClassesFromAssemblies(string pathToAssemblies);
    }
}