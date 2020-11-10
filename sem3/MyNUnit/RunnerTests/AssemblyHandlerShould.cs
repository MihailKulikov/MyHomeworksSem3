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
        public void Return_Nothing_When_There_Are_No_Unloaded_Assemblies_In_Directory()
        {
            Assert.That(assemblyHandler.GetTestClassesFromAssemblies("."), Is.Empty);
        }
    }
}