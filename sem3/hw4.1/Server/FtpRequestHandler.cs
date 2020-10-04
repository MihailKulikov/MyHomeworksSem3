using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    public static class FtpRequestHandler
    {
        private const string InputCommandPattern = "^[12] ..*";
        private const string ErrorResponse = "-1";

        public static async Task<string> HandleRequest(string? request)
        {
            if (request == null)
            {
                return ErrorResponse;
            }

            if (!Regex.IsMatch(request, InputCommandPattern))
            {
                return ErrorResponse;
            }

            var commandCode = request[0];
            var path = request.Substring(2);
            return commandCode switch
            {
                '1' => List(path),
                '2' => await Get(path),
                _ => ErrorResponse
            };
        }

        private static string List(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                var filePaths = directoryInfo.GetFiles().Select(fileInfo =>
                    Path.GetRelativePath(Directory.GetCurrentDirectory(), fileInfo.FullName)).ToList();
                var directoryPaths = directoryInfo.GetDirectories().Select(info =>
                    Path.GetRelativePath(Directory.GetCurrentDirectory(), info.FullName)).ToList();
                var stringBuilder = new StringBuilder($"{filePaths.Count + directoryPaths.Count} ");
                stringBuilder.AppendJoin(" false ", filePaths);
                if (filePaths.Count != 0)
                {
                    stringBuilder.Append(" false ");
                }

                stringBuilder.AppendJoin(" true ", directoryPaths);
                if (directoryPaths.Count != 0)
                {
                    stringBuilder.Append(" true ");
                }

                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                return ErrorResponse;
            }
        }

        private static async Task<string> Get(string path)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(path);
                var stringBuilder = new StringBuilder($"{bytes.Length }");
                stringBuilder.AppendJoin(' ', bytes);

                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                return ErrorResponse;
            }
        }
    }
}