using System.IO;
using System.Linq;
using System.Text;

namespace Test2
{
    public static class CheckSumHandler
    {
        public static byte[] GetCheckSum(DirectoryInfo directoryInfo)
        {
            var checkSumOfFilesInDirectory = directoryInfo
                .EnumerateFiles()
                .OrderBy(info => info.Name)
                .Select(GetCheckSumOfSingleFile)
                .Select(Encoding.Default.GetString)
                .ToList();

            var subdirectoriesCheckSums = directoryInfo
                .GetDirectories()
                .OrderBy(info => info.Name)
                .Select(GetCheckSum)
                .Select(Encoding.Default.GetString)
                .ToList();

            using var md5 = System.Security.Cryptography.MD5.Create();
            
            var stringBuilder = new StringBuilder(directoryInfo.Name);
            stringBuilder.Append(string.Join("", checkSumOfFilesInDirectory));
            stringBuilder.Append(string.Join("", subdirectoriesCheckSums));
            
            var resultSum = md5.ComputeHash(Encoding.Default.GetBytes(stringBuilder.ToString()));

            return resultSum;
        }

        public static byte[] MultithreadedGetCheckSum(DirectoryInfo directoryInfo)
        {
            var checkSumOfFilesInDirectory = directoryInfo
                .EnumerateFiles()
                .AsParallel()
                .OrderBy(info => info.Name)
                .Select(GetCheckSumOfSingleFile)
                .Select(Encoding.Default.GetString)
                .ToList();

            var subdirectoriesCheckSums = directoryInfo
                .GetDirectories()
                .AsParallel()
                .OrderBy(info => info.Name)
                .Select(GetCheckSum)
                .Select(Encoding.Default.GetString)
                .ToList();

            using var md5 = System.Security.Cryptography.MD5.Create();
            
            var stringBuilder = new StringBuilder(directoryInfo.Name);
            stringBuilder.Append(string.Join("", checkSumOfFilesInDirectory));
            stringBuilder.Append(string.Join("", subdirectoriesCheckSums));
            
            var resultSum = md5.ComputeHash(Encoding.Default.GetBytes(stringBuilder.ToString()));

            return resultSum;
        }

        private static byte[] GetCheckSumOfSingleFile(FileInfo fileInfo)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            using var fileStream = fileInfo.Open(FileMode.Open);
            fileStream.Position = 0;
            var hashValue = md5.ComputeHash(fileStream);

            return hashValue;
        }
    }
}