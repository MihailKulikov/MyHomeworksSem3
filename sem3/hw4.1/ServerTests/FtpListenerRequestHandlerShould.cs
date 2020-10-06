using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Server;

namespace ServerTests
{
    public class FtpListenerRequestHandlerShould
    {
        private const string ErrorResponse = "-1";
        private readonly string patternForSplittingListResponse = $"( {true} )|( {false} )|( {true})|( {false})";
        private FtpListenerRequestHandler ftpListenerRequestHandler;
        private MemoryStream testStream;
        private StreamReader reader;

        [SetUp]
        public void SetUp()
        {
            testStream = new MemoryStream();
            reader = new StreamReader(testStream);
            ftpListenerRequestHandler = new FtpListenerRequestHandler(testStream);
        }

        private (int size, (string name, bool isDirectory)[] paths) ParseServerResponse(string response)
        {
            var splittedData = response.Split(' ', 2);
            var size = int.Parse(splittedData[0]);
            var nameAndIsDirStrings = Regex.Split(splittedData[1], patternForSplittingListResponse);
            var result = new (string name, bool isDirectory)[size];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (nameAndIsDirStrings[i * 2], bool.Parse(nameAndIsDirStrings[i * 2 + 1]));
            }

            return (size, result);
        }

        [TestCase("3 DELETE")]
        [TestCase("")]
        [TestCase("Destroy server")]
        [TestCase("1 ???incorrectPath???")]
        [TestCase("1 <<haha>>")]
        [Test]
        public async Task WriteLine_ErrorResponse_When_Request_Is_Invalid(string invalidRequest)
        {
            await ftpListenerRequestHandler.HandleRequestAsync(invalidRequest);
            testStream.Position = 0;

            Assert.That(await reader.ReadLineAsync(), Is.EqualTo(ErrorResponse));
        }

        [TestCase("2 ???incorrectName???")]
        [TestCase("2 <<haha>>")]
        public async Task Write_ErrorResponse_When_Try_To_Get_With_Invalid_Name(string invalidName)
        {
            var resultBuffer = new char[3];

            await ftpListenerRequestHandler.HandleRequestAsync(invalidName);
            testStream.Position = 0;

            await reader.ReadAsync(resultBuffer, 0, 3);
            Assert.That(resultBuffer, Is.EquivalentTo(new[] {'-', '1', '\0'}));
        }

        [Test]
        public async Task WriteLine_CorrectResponse_To_List_Command()
        {
            const string initialDirectoryPath = "directory";
            var testDirectoryPath = @$"{initialDirectoryPath}\testDirectory";
            var testFilePath = @$"{initialDirectoryPath}\testFile.txt";
            var initialDirectoryInfo = Directory.CreateDirectory(initialDirectoryPath);
            Directory.CreateDirectory(testDirectoryPath);
            var fileStream = File.Create(testFilePath);

            await ftpListenerRequestHandler.HandleRequestAsync($"1 {initialDirectoryPath}");
            testStream.Position = 0;

            var (size, paths) = ParseServerResponse(await reader.ReadLineAsync());
            fileStream.Close();
            initialDirectoryInfo.Delete(true);
            Assert.That(size, Is.EqualTo(2));
            Assert.That(paths, Is.EquivalentTo(new[] {(testDirectoryPath, true), (testFilePath, false)}));
        }

        [Test]
        public async Task Write_CorrectResponse_To_Get_Command()
        {
            const string testFilePath = "testFile.txt";
            const string message = "Hello, world!";
            var fileStream = File.Create(testFilePath);
            var fileStreamWriter = new StreamWriter(fileStream) {AutoFlush = true};
            await fileStreamWriter.WriteLineAsync(message);
            fileStream.Close();

            await ftpListenerRequestHandler.HandleRequestAsync($"2 {testFilePath}");
            testStream.Position = 0;

            var actualMessage = await reader.ReadLineAsync();
            var buffer = new char[1];
            await reader.ReadAsync(buffer, 0, 1);
            Assert.That(actualMessage, Is.EqualTo($"{message.Length + Environment.NewLine.Length} {message}"));
            Assert.That(buffer[0], Is.EqualTo('\0'));

            File.Delete(testFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            ftpListenerRequestHandler.Dispose();
            testStream.Dispose();
            reader.Dispose();
        }
    }
}