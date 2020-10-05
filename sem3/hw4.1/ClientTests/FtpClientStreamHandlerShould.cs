using System.IO;
using Client;
using Moq;
using NUnit.Framework;

namespace ClientTests
{
    public class FtpClientStreamHandlerShould
    {
        private FtpClientStreamHandler handler;
        private Mock<Stream> streamMock;

        [SetUp]
        public void SetUp()
        {
            streamMock = new Mock<Stream>();
            handler = new FtpClientStreamHandler(streamMock.Object);
        }

        [Test]
        public void ThrowDirectoryNotFoundException_When_Server_Returns_MinusOne()
        {
            
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            handler.Dispose();
        }
    }
}