using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OnlineChat;
using Moq;

namespace OnlineChatTests
{
    public class Tests
    {
        private StreamHandler streamHandler;
        private MemoryStream memoryStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private Mock<TextWriter> textWriterMock;
        private Mock<TextReader> textReaderMock;
        
        [SetUp]
        public void Setup()
        {
            memoryStream = new MemoryStream();
            streamReader = new StreamReader(memoryStream);
            streamWriter = new StreamWriter(memoryStream) {AutoFlush = true};
            textWriterMock = new Mock<TextWriter>();
            textReaderMock = new Mock<TextReader>();
            streamHandler = new StreamHandler(memoryStream, textReaderMock.Object, textWriterMock.Object);
        }

        [Test]
        public async Task EndReadingWhenStreamContainsExit()
        {
            await streamWriter.WriteLineAsync("exit");
            memoryStream.Position = 0;
            
            await streamHandler.ReadFromStream();
            
            textWriterMock.Verify(writer => writer.WriteLineAsync("exit"), Times.Once);
        }

        [Test]
        public async Task EndWritingWhenUserInputIsExit()
        {
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync("exit").Verifiable();

            await streamHandler.WriteToStream();

            memoryStream.Position = 0;
            textReaderMock.Verify();
            Assert.That(await streamReader.ReadLineAsync(), Is.EqualTo("exit"));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            streamHandler.Dispose();
        }
    }
}