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
        private byte[] buffer;
        private MemoryStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        [SetUp]
        public void SetUp()
        {
            buffer = new byte[BufferLength];
            stream = new MemoryStream(buffer, 0, buffer.Length);
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
            
            Assert.That(testBuffer, Is.EquivalentTo(new []{0, 54}));
        }

        [Test]
        public void Throw_ArgumentException_When_Try_To_CopyTo_With_Negative_Count()
        {
            const string exceptionMessage = "Count should be not negative.";

            Assert.That(async () => await streamHandler.CopyToAsync(It.IsAny<Stream>(), -1),
                Throws.ArgumentException.And.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task CopyToAsync()
        {
            const string value = "Something";
            await writer.WriteLineAsync(value);
            using var destinationReader = new StreamReader(new MemoryStream());
            stream.Position = 0;

            await streamHandler.CopyToAsync(destinationReader.BaseStream, value.Length + 1);
            destinationReader.BaseStream.Position = 0;

            Assert.That(await destinationReader.ReadLineAsync(), Is.EqualTo(value));
        }
        // [Test]
        // public void Throw_DirectoryNotFoundException_When_Server_Returns_MinusOne_In_Response_To_List_Command()
        // {
        //     const string serverResponse = "-1";
        //     const string clientRequest = "something";
        //     streamHandlerMock.Setup(handler => handler.ReadLineAsync()).ReturnsAsync(serverResponse).Verifiable();
        //
        //     Assert.That(async () => await requestHandler.List(clientRequest), Throws.TypeOf<DirectoryNotFoundException>());
        //     streamHandlerMock.Verify();
        //     streamHandlerMock.Verify(handler => handler.WriteLineAsync(clientRequest), Times.Once);
        //     streamHandlerMock.VerifyNoOtherCalls();
        // }
        //
        // [Test]
        // public void Return_Result_According_To_Correct_Server_Response_To_List_Command()
        // {
        //     const string serverResponse = "2 FileName false DirectoryName true";
        //     const string clientRequest = "something";
        // }

        [TearDown]
        public void TearDown()
        {
            streamHandler.Dispose();
        }
    }
}