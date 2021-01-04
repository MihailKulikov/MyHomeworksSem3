using System.IO;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using NUnit.Framework;

namespace RunnerTests
{
    public class AssemblyHandlerShould
    {
        private IAssemblyHandler assemblyHandler;

        [SetUp]
        public void SetUp()
        {
            assemblyHandler = new AssemblyHandler();
        }    
        
        [Test]
        public void Return_Nothing_When_There_Are_No__Assemblies_In_Directory()
        {
            const string pathToDirectory = @".\TestDirectory";
            var directoryWithoutAssemblies = Directory.CreateDirectory(pathToDirectory);
            try
            {
                Assert.That(assemblyHandler.GetTestClassesFromAssemblies(@".\TestDirectory"), Is.Empty);
            }
            finally
            {
                directoryWithoutAssemblies.Delete();
            }
        }

        [Test]
        public void Throw_DirectoryNotFoundException_When_There_Is_No_Directory_With_Such_Path()
        {
            Assert.That(() => assemblyHandler.GetTestClassesFromAssemblies("```-"),
                Throws.Exception.TypeOf<DirectoryNotFoundException>());
        }
    }
}