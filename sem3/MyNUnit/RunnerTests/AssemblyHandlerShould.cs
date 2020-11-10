using System.IO;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using NUnit.Framework;

namespace RunnerTests
{
    public class AssemblyHandlerShould
    {
        private IAssemblyHandler assemblyHandler;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }
        
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

        // [Test]
        // public void Returns_Collection_Of_Test_Class_Wrappers_Of_This_Test_Project()
        // {
        //     var testClasses = assemblyHandler.GetTestClassesFromAssemblies(@".\").ToList();
        //
        //     Assert.That(testClasses.Count, Is.EqualTo(2));
        //     var multipleBeforeTestAfterHandlerShould =
        //         testClasses[0].ClassType == typeof(MultipleBeforeTestAfterHandlerShould)
        //             ? testClasses[0]
        //             : testClasses[1];
        //     var testHandlerShould = 
        //         testClasses[0].ClassType == typeof(TestHandlerShould)
        //             ? testClasses[0]
        //             : testClasses[1];
        //     Assert.That(multipleBeforeTestAfterHandlerShould.AfterMethodInfos, Is.Empty);
        //     Assert.That(multipleBeforeTestAfterHandlerShould.BeforeMethodInfos, Is.Empty);
        //     Assert.That(multipleBeforeTestAfterHandlerShould.TestClassInstance,
        //         Is.TypeOf<MultipleBeforeTestAfterHandlerShould>());
        //     Assert.That(multipleBeforeTestAfterHandlerShould.AfterClassMethodInfos, Is.Empty);
        //     Assert.That(multipleBeforeTestAfterHandlerShould.BeforeClassMethodInfos, Is.Empty);
        //     Assert.That(multipleBeforeTestAfterHandlerShould.TestMethodInfos,
        //         Is.EquivalentTo(new[]
        //         {
        //             multipleBeforeTestAfterHandlerShould.ClassType.GetMethod(nameof(MultipleBeforeTestAfterHandlerShould
        //                 .Test))
        //         }));
        //     Assert.That(testHandlerShould.AfterMethodInfos, Is.Empty);
        //     Assert.That(testHandlerShould.BeforeMethodInfos, Is.Empty);
        //     Assert.That(testHandlerShould.TestClassInstance, Is.TypeOf<TestHandlerShould>());
        //     Assert.That(testHandlerShould.AfterClassMethodInfos, Is.Empty);
        //     Assert.That(testHandlerShould.BeforeClassMethodInfos, Is.Empty);
        //     Assert.That(testHandlerShould.TestMethodInfos, Is.EquivalentTo(new[]
        //     {
        //         testHandlerShould.ClassType.GetMethod(nameof(TestHandlerShould.IgnoredTest)),
        //         testHandlerShould.ClassType.GetMethod(nameof(TestHandlerShould.ThrowsExpectedException)),
        //         testHandlerShould.ClassType.GetMethod(nameof(TestHandlerShould.ThrowUnexpectedException)),
        //         testHandlerShould.ClassType.GetMethod(nameof(TestHandlerShould.DoesNotThrowExpectedException)),
        //         testHandlerShould.ClassType.GetMethod(nameof(TestHandlerShould.GoodTest))
        //     }));
        // }
    }
}