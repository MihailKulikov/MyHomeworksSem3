using System.IO;
using System.Threading.Tasks;
using Client;
using Client.Interfaces;
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
        private const string IncorrectInputMessage = "Incorrect input :) Try again.";
        private const string ExitCode = "qa!";

        private const string ListCommandInformation =
            "List, request format:\n\t1 <path: String>\n\tpath - path to the directory relative to where the server is running";

        private const string GetCommandInformation =
            "Get, request format:\n\t2 <path: String>\n\tpath - path to the file relative to where the server is running";

        private readonly string exitCodeInformation = $"Exit, request format:\n\t{ExitCode}";

        private void VerifyIntroductionWasWritten()
        {
            textWriterMock.Verify(writer => writer.WriteLineAsync("Connection established."), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(ListCommandInformation), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(GetCommandInformation), Times.Once);
            textWriterMock.Verify(writer => writer.WriteLineAsync(exitCodeInformation), Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            ftpClientMock = new Mock<IFtpClient>();
            textWriterMock = new Mock<TextWriter>();
            textReaderMock = new Mock<TextReader>();
            cui = new CuiOfFtpClient(ftpClientMock.Object, textWriterMock.Object, textReaderMock.Object);
        }

        [Test]
        public async Task ShowIntroductionAndExitAfterReadingExitCode()
        {
            textReaderMock.Setup(reader => reader.ReadLineAsync()).ReturnsAsync(ExitCode).Verifiable();

            await cui.Run();

            textReaderMock.Verify();
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Show_IncorrectInputMessage_When_Input_Is_Incorrect()
        {
            textReaderMock.SetupSequence(reader => reader.ReadLineAsync())
                .ReturnsAsync("something wrong")
                .ReturnsAsync(ExitCode);

            await cui.Run();

            textReaderMock.Verify(reader => reader.ReadLineAsync(), Times.Exactly(2));
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.Verify(writer => writer.WriteLineAsync(IncorrectInputMessage));
            textWriterMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Show_Result_Of_Correct_List_Command()
        {
            textReaderMock.SetupSequence(reader => reader.ReadLineAsync())
                .ReturnsAsync("1 someDirectory")
                .ReturnsAsync(ExitCode);
            ftpClientMock.Setup(client => client.ListAsync("someDirectory"))
                .ReturnsAsync(new (string name, bool isDirectory)[] {("file", false), ("directory", true)})
                .Verifiable();

            await cui.Run();

            ftpClientMock.Verify();
            ftpClientMock.VerifyNoOtherCalls();
            textReaderMock.Verify(reader => reader.ReadLineAsync(), Times.Exactly(2));
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.Verify(writer => writer.WriteLineAsync("file False"));
            textWriterMock.Verify(writer => writer.WriteLineAsync("directory True"));
            textWriterMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Show_ExceptionMessage_When_Directory_Not_Found()
        {
            const string exceptionMessage = "Directory not found.";
            textReaderMock.SetupSequence(reader => reader.ReadLineAsync())
                .ReturnsAsync("1 someDirectory")
                .ReturnsAsync(ExitCode);
            ftpClientMock.Setup(client => client.ListAsync("someDirectory"))
                .ThrowsAsync(new DirectoryNotFoundException(exceptionMessage))
                .Verifiable();

            await cui.Run();

            ftpClientMock.Verify();
            ftpClientMock.VerifyNoOtherCalls();
            textReaderMock.Verify(reader => reader.ReadLineAsync(), Times.Exactly(2));
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.Verify(writer => writer.WriteLineAsync(exceptionMessage));
            textWriterMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Task_Show_Result_Of_Correct_Get_Command()
        {
            const string resultOfGetCommand = "some path";
            var expectedMessage = $"File successfully downloaded to {resultOfGetCommand}";
            textReaderMock.SetupSequence(reader => reader.ReadLineAsync())
                .ReturnsAsync("2 someFile")
                .ReturnsAsync(ExitCode);
            ftpClientMock.Setup(client => client.GetAsync("someFile"))
                .ReturnsAsync(resultOfGetCommand)
                .Verifiable();

            await cui.Run();

            ftpClientMock.Verify();
            ftpClientMock.VerifyNoOtherCalls();
            textReaderMock.Verify(reader => reader.ReadLineAsync(), Times.Exactly(2));
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.Verify(writer => writer.WriteLineAsync(expectedMessage));
            textWriterMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Show_ExceptionMessage_When_File_Not_Found()
        {
            const string exceptionMessage = "File not found.";
            textReaderMock.SetupSequence(reader => reader.ReadLineAsync())
                .ReturnsAsync("2 someFile")
                .ReturnsAsync(ExitCode);
            ftpClientMock.Setup(client => client.GetAsync("someFile"))
                .ThrowsAsync(new FileNotFoundException(exceptionMessage))
                .Verifiable();

            await cui.Run();

            ftpClientMock.Verify();
            ftpClientMock.VerifyNoOtherCalls();
            textReaderMock.Verify(reader => reader.ReadLineAsync(), Times.Exactly(2));
            textReaderMock.VerifyNoOtherCalls();
            VerifyIntroductionWasWritten();
            textWriterMock.Verify(writer => writer.WriteLineAsync(exceptionMessage));
            textWriterMock.VerifyNoOtherCalls();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            cui.Dispose();
        }
    }
}