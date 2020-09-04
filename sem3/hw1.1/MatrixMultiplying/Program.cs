using System;

namespace MatrixMultiplying
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(
                "Percentage ration of the single thread working time to the multiple thread working time:");
            
            for (var i = 10; i <= 300; i += 10)
            {
                var firstMatrix = Matrix.GenerateRandomMatrix(i, i);
                var secondMatrix = Matrix.GenerateRandomMatrix(i, i);

                var singleThreadElapsedTime =
                    TimeGauge.CalculateElapsedTime(firstMatrix.MultiplyWith, secondMatrix, out _);
                var multipleThreadElapsedTime = TimeGauge.CalculateElapsedTime(firstMatrix.ParallelMultiplyWith,
                    secondMatrix, out _);

                Console.WriteLine($"{i}*{i} matrices: {100 * multipleThreadElapsedTime.TotalMilliseconds / singleThreadElapsedTime.TotalMilliseconds}");
            }
        }
    }
}