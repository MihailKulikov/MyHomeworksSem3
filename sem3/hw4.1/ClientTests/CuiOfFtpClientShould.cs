using System.IO;
using System.Threading.Tasks;
using Client;
using NUnit.Framework;
using Moq;

namespace ClientTests
{
    public class CuiOfFtpClientShould
    {
        private CuiOfFtpClient cui;
        private Mock<IFtpClient> ftpClientMock;
        private Mock<TextWriter> textWriterMock;
        private Mock<TextReader> textReaderMock;
        private const string ExitCode = "qa!";
        private const string ListCommandInformation =
            "List, request format:\n\t1 <path: String>\n\tpath - path to the directory relative to where the server is running";

        private const string GetCommandInformation =
            "Get, request format:\n\t2 <path: String>\n\tpath - path to the file relative to where the server is running";

        private readonly string exitCodeInformation = $"Exit, request format:\n\t{ExitCode}";
        
        [SetUp]
        public void SetUp()
        {
            ftpClientMock = new Mock<IFtpClient>();
            textWriterMock = new Mock<TextWriter>();
            textReaderMock = new Mock<TextReader>();
            cui = new CuiOfFtpClient(ftpClientMock.Object, textWriterMock.Object, textReaderMock.Object);
        }

        [Test]
        public async Task ShowIntroduction()
        {
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync(ExitCode);

            await cui.Run();
            
            textWriterMock.Verify(writer => writer.WriteLineAsync("Connection established."), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(ListCommandInformation), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(GetCommandInformation), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(exitCodeInformation), Times.Once);
        }
    }
}