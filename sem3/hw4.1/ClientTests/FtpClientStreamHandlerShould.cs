using System;
using System.IO;
using System.Threading.Tasks;
using Client;
using Moq;
using NUnit.Framework;

namespace ClientTests
{
    public class FtpClientStreamHandlerShould
    {
        private FtpClientStreamHandler streamHandler;
        private const int BufferLength = 4096;
        private MemoryStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        [SetUp]
        public void SetUp()
        {
            stream = new MemoryStream();
            streamHandler = new FtpClientStreamHandler(stream);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) {AutoFlush = true};
        }

        [Test]
        public async Task WriteLineAsync()
        {
            const string expectedResult = "something";

            await streamHandler.WriteLineAsync(expectedResult);
            stream.Position = 0;

            Assert.That(await reader.ReadLineAsync(), Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task ReadLineAsync()
        {
            const string expectedResult = "something";

            await writer.WriteLineAsync(expectedResult);
            stream.Position = 0;

            Assert.That(await streamHandler.ReadLineAsync(), Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task ReadAsync()
        {
            var testBuffer = new byte[2];
            await writer.WriteAsync(Convert.ToChar(54));
            stream.Position = 0;

            await streamHandler.ReadAsync(testBuffer, 1, 1);

            Assert.That(testBuffer, Is.EquivalentTo(new[] {0, 54}));
        }

        [Test]
        public void Throw_ArgumentException_When_Try_To_CopyTo_With_Negative_Count()
        {
            const string exceptionMessage = "Count should be not negative.";

            Assert.That(async () => await streamHandler.CopyToAsync(It.IsAny<Stream>(), -1),
                Throws.ArgumentException.And.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task CopyToAsync_With_Size_Smaller_Than_Buffer()
        {
            const string value = "Something";
            await writer.WriteLineAsync(value);
            using var destinationReader = new StreamReader(new MemoryStream());
            stream.Position = 0;

            await streamHandler.CopyToAsync(destinationReader.BaseStream, value.Length + 1);
            destinationReader.BaseStream.Position = 0;

            Assert.That(await destinationReader.ReadLineAsync(), Is.EqualTo(value));
        }

        [Test]
        public async Task CopyToAsync_With_Size_Bigger_Than_Buffer()
        {
            var value = string.Empty.PadRight(BufferLength * 2 + 1, 'a');
            await writer.WriteLineAsync(value);
            using var destinationReader = new StreamReader(new MemoryStream());
            stream.Position = 0;

            await streamHandler.CopyToAsync(destinationReader.BaseStream, value.Length + 1);
            destinationReader.BaseStream.Position = 0;

            Assert.That(await destinationReader.ReadLineAsync(), Is.EqualTo(value));
        }

        [TearDown]
        public void TearDown()
        {
            streamHandler.Dispose();
            stream.Dispose();
            writer.Dispose();
            reader.Dispose();
        }
    }
}