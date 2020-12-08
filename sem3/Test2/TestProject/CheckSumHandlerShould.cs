using System.IO;
using NUnit.Framework;
using Test2;

namespace TestProject
{
    public class CheckSumHandlerShould
    {
        private DirectoryInfo testDirectoryInfo;
        private const string PathToTestDirectory = "./TestDirectory";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            testDirectoryInfo = Directory.CreateDirectory(PathToTestDirectory);
            var subdirectory = testDirectoryInfo.CreateSubdirectory("TestSubDirectory");
            var testFile = new FileInfo(Path.Join(testDirectoryInfo.FullName, "testFile.txt"));
            using var testFileStream = testFile.Create();
            testFileStream.Write(new byte[] {42, 42, 42});
            var testFileInSubdirectory = new FileInfo(Path.Join(subdirectory.FullName, "testFile.txt"));
            using var testFileInSubdirectoryStream = testFileInSubdirectory.Create();
            testFileInSubdirectoryStream.Write(new byte[] {54, 54, 54});
        }

        [Test]
        public void SingleThreadedCheckSumCalculateSameValueOfSameDirectory()
        {
            var firstCheckSum = CheckSumHandler.GetCheckSum(testDirectoryInfo);
            var secondCheckSum = CheckSumHandler.GetCheckSum(testDirectoryInfo);

            Assert.That(firstCheckSum, Is.EqualTo(secondCheckSum));
        }

        [Test]
        public void MultiThreadedCheckSumCalculateSameValueOfSameDirectory()
        {
            var firstCheckSum = CheckSumHandler.MultithreadedGetCheckSum(testDirectoryInfo);
            var secondCheckSum = CheckSumHandler.MultithreadedGetCheckSum(testDirectoryInfo);
        
            Assert.That(firstCheckSum, Is.EqualTo(secondCheckSum));
        }

        [Test]
        public void SingThreadedAndMultThreadedCheckSumAreSame()
        {
            var singleCheckSum = CheckSumHandler.GetCheckSum(testDirectoryInfo);
            var multiCheckSum = CheckSumHandler.MultithreadedGetCheckSum(testDirectoryInfo);

            Assert.That(singleCheckSum, Is.EqualTo(multiCheckSum));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            testDirectoryInfo.Delete(true);
        }
    }
}