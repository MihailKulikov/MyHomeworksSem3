using System;
using System.IO;
using System.Threading.Tasks;
using Client;
using Client.Interfaces;
using Moq;
using NUnit.Framework;

namespace ClientTests
{
    public class FtpClientShould
    {
        private FtpClient ftpClient;
        private Mock<IFtpClientStreamHandler> streamHandlerMock;

        [SetUp]
        public void SetUp()
        {
            streamHandlerMock = new Mock<IFtpClientStreamHandler>();
            ftpClient = new FtpClient(streamHandlerMock.Object);
        }

        [Test]
        public void Throw_DirectoryNotFoundException_When_Server_Returns_MinusOne_In_Response_To_List_Command()
        {
            const string serverResponse = "-1";
            const string clientRequest = "something";
            streamHandlerMock.Setup(handler => handler.ReadLineAsync())
                .ReturnsAsync(serverResponse)
                .Verifiable();

            Assert.That(async () => await ftpClient.List(clientRequest), Throws.TypeOf<DirectoryNotFoundException>());
            streamHandlerMock.Verify();
            streamHandlerMock.Verify(handler => handler.WriteLineAsync($"1 {clientRequest}"), Times.Once);
            streamHandlerMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Return_Result_According_To_Correct_Server_Response_To_List_Command()
        {
            var serverResponse = $"2 FileName {false} DirectoryName {true}";
            const string clientRequest = "something";
            streamHandlerMock.Setup(handler => handler.ReadLineAsync())
                .ReturnsAsync(serverResponse)
                .Verifiable();

            Assert.That(await ftpClient.List(clientRequest),
                Is.EquivalentTo(new[] {("FileName", false), ("DirectoryName", true)}));
            streamHandlerMock.Verify();
            streamHandlerMock.Verify(handler => handler.WriteLineAsync($"1 {clientRequest}"), Times.Once);
            streamHandlerMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Throw_FileNotFoundException_When_Server_Returns_MinusOne_In_Response_To_Get_Command()
        {
            const string clientRequest = "something";
            streamHandlerMock.Setup(handler => handler.ReadAsync(It.IsAny<byte[]>(), 0, 2))
                .Callback((byte[] buffer, int offset, int count) =>
                {
                    buffer[0] = Convert.ToByte('-');
                    buffer[1] = Convert.ToByte('1');
                });

            Assert.That(async () => await ftpClient.Get(clientRequest), Throws.TypeOf<FileNotFoundException>());
            streamHandlerMock.Verify(handler => handler.ReadAsync(It.IsAny<byte[]>(), 0, 2), Times.Once);
            streamHandlerMock.Verify(handler => handler.WriteLineAsync($"2 {clientRequest}"), Times.Once);
            streamHandlerMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Return_Path_To_File_In_Which_Received_Data_From_Server_In_Response_To_Get_Command()
        {
            const string clientRequest = "something";
            var bytesRead = 0;
            var expectedBuffer = new[] {Convert.ToByte('1'), Convert.ToByte('0'), Convert.ToByte(' ')};
            FileStream usedByClientFileStream = null;
            streamHandlerMock.Setup(handler => handler.ReadAsync(It.IsAny<byte[]>(), It.Is<int>(i => i <= 2 && i >= 0),
                    It.Is<int>(i => i == 1 || i == 2)))
                .Callback((byte[] buffer, int offset, int count) =>
                {
                    for (var i = offset; i - offset < count; i++)
                    {
                        buffer[i] = expectedBuffer[i];
                    }

                    bytesRead += count;
                });
            streamHandlerMock.Setup(handler => handler.CopyToAsync(It.IsAny<FileStream>(), 10))
                .Callback((Stream stream, long size) => { usedByClientFileStream = (FileStream) stream; });

            Assert.That(await ftpClient.Get(clientRequest),
                Is.EqualTo(Path.GetRelativePath(Directory.GetCurrentDirectory(), usedByClientFileStream.Name)));
            streamHandlerMock.Verify(handler => handler.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.AtLeastOnce);
            streamHandlerMock.Verify(handler => handler.WriteLineAsync($"2 {clientRequest}"), Times.Once);
            streamHandlerMock.Verify(handler => handler.CopyToAsync(It.IsAny<Stream>(), It.IsAny<long>()), Times.Once);
            streamHandlerMock.VerifyNoOtherCalls();
            Assert.That(bytesRead, Is.EqualTo(expectedBuffer.Length));

            File.Delete(usedByClientFileStream.Name);
        }

        [TearDown]
        public void TearDown()
        {
            ftpClient.Dispose();
        }
    }
}