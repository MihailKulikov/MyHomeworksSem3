using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Represents request handler for SimpleFTP server.
    /// </summary>
    public class FtpListenerRequestHandler : IDisposable
    {
        private const string InputCommandPattern = "^[12] ..*";
        private const string ErrorResponse = "-1";
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        /// <summary>
        /// Initialize a new instance of the <see cref="FtpListenerRequestHandler"/> class that works with specified stream. 
        /// </summary>
        /// <param name="stream">Specified stream.</param>
        public FtpListenerRequestHandler(Stream stream)
        {
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) {AutoFlush = true};
        }

        /// <summary>
        /// Handles request from the SimpleFTP client.
        /// </summary>
        /// <param name="request">Request from SimpleFTP client.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleRequestAsync(string request)
        {
            if (!Regex.IsMatch(request, InputCommandPattern))
            {
                await writer.WriteLineAsync(ErrorResponse);
                return;
            }

            var commandCode = request[0];
            var path = request.Substring(2);
            switch (commandCode)
            {
                case '1':
                    await List(path);
                    return;
                case '2':
                    await Get(path);
                    return;
            }
        }

        private async Task List(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                var filePaths = directoryInfo.GetFiles().Select(fileInfo =>
                    Path.GetRelativePath(Directory.GetCurrentDirectory(), fileInfo.FullName)).ToList();
                var directoryPaths = directoryInfo.GetDirectories().Select(info =>
                    Path.GetRelativePath(Directory.GetCurrentDirectory(), info.FullName)).ToList();
                var stringBuilder = new StringBuilder($"{filePaths.Count + directoryPaths.Count} ");
                stringBuilder.AppendJoin($" {false.ToString()} ", filePaths);
                if (filePaths.Count != 0)
                {
                    stringBuilder.Append($" {false}");
                }

                stringBuilder.AppendJoin($" {true} ", directoryPaths);
                if (directoryPaths.Count != 0)
                {
                    stringBuilder.Append($" {true}");
                }

                await writer.WriteLineAsync(stringBuilder.ToString());
            }
            catch (Exception)
            {
                await writer.WriteLineAsync(ErrorResponse);
            }
        }

        private async Task Get(string path)
        {
            try
            {
                var fileStream = new FileStream(path, FileMode.Open);
                await writer.WriteAsync(new FileInfo(path).Length + " ");
                await fileStream.CopyToAsync(writer.BaseStream);
                fileStream.Close();
            }
            catch (Exception)
            {
                await writer.WriteAsync(ErrorResponse[0]);
                await writer.WriteAsync(ErrorResponse[1]);
            }
        }

        /// <summary>
        /// Releases all resources used by <see cref="FtpListenerRequestHandler"/> object.
        /// </summary>
        public void Dispose()
        {
            reader.Dispose();
            writer.Dispose();
        }
    }
}