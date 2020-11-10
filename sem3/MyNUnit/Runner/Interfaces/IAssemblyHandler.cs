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
        /// If the directory does not exist, returns an empty collection.
        /// </summary>
        /// <param name="pathToAssemblies">Specified path to the directory that contains assemblies.</param>
        /// <returns>Instances of the <see cref="ITestClassWrapper"/> that represent test classes.</returns>
        IEnumerable<ITestClassWrapper> GetTestClassesFromAssemblies(string pathToAssemblies);
    }
}