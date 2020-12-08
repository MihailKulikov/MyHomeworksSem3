using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Test2
{
    internal static class Program
    {
        private const int NumberOfRuns = 10;
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No directory selected.");
                return;
            }

            var pathToDirectory = args[0];
            if (Directory.Exists(pathToDirectory))
            {
                var directoryInfo = new DirectoryInfo(pathToDirectory);
                var multithreadedElapsedTimeInMilliseconds = new List<long>();
                var singlethreadedElapsedTimeInMilliseconds = new List<long>();

                for (var i = 0; i < NumberOfRuns; i++)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var checkSum = CheckSumHandler.GetCheckSum(directoryInfo);
                    stopwatch.Stop();
                    singlethreadedElapsedTimeInMilliseconds.Add(stopwatch.ElapsedMilliseconds);
                    stopwatch.Reset();
                    stopwatch.Start();
                    var newCheckSum = CheckSumHandler.MultithreadedGetCheckSum(directoryInfo);
                    stopwatch.Stop();
                    multithreadedElapsedTimeInMilliseconds.Add(stopwatch.ElapsedMilliseconds);
                }

                Console.WriteLine(
                    $"Average running time of a single-threaded program: {CalculateAverage(singlethreadedElapsedTimeInMilliseconds)}");
                Console.WriteLine(
                    $"Standard deviation of a single-threaded program: {CalculateStandardDeviation(singlethreadedElapsedTimeInMilliseconds)}");
                Console.WriteLine(
                    $"Average running time of a multi-threaded program: {CalculateAverage(multithreadedElapsedTimeInMilliseconds)}");
                Console.WriteLine(
                    $"Standard deviation of a single-threaded program: {CalculateStandardDeviation(multithreadedElapsedTimeInMilliseconds)}");

                var resultCheckSum = CheckSumHandler.MultithreadedGetCheckSum(directoryInfo);
                var hex = new StringBuilder();
                foreach (var b in resultCheckSum)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                Console.WriteLine(hex.ToString());
            }
            else
            {
                Console.WriteLine("The directory specified could not be found.");
            }
        }

        private static double CalculateAverage(IReadOnlyCollection<long> samples) =>
            samples.Sum() / Convert.ToDouble(samples.Count);

        private static double CalculateStandardDeviation(IReadOnlyCollection<long> samples)
        {
            var average = CalculateAverage(samples);
            return Math.Sqrt(samples.Select(sample => Math.Pow(Convert.ToDouble(sample) - average, 2)).Sum() /
                             samples.Count);
        }
    }
}