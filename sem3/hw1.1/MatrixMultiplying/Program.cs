using System;

namespace MatrixMultiplying
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(
                "Percentage ration of the single thread working time to the multiple thread working time:");
            
            for (var i = 10; i <= 500; i += 10)
            {
                var firstMatrix = Matrix.GenerateRandomMatrix(i, i);
                var secondMatrix = Matrix.GenerateRandomMatrix(i, i);

                var myMultipleThreadElapsedTime =
                    TimeGauge.CalculateElapsedTime(firstMatrix.MyVersionOfParallelMultiplyWith, secondMatrix, out _);
                var multipleThreadElapsedTime = TimeGauge.CalculateElapsedTime(firstMatrix.ParallelForMultiplyWith,
                    secondMatrix, out _);

                Console.WriteLine($"{i}*{i} matrices: {100 * multipleThreadElapsedTime.TotalMilliseconds / myMultipleThreadElapsedTime.TotalMilliseconds}");
            }
        }
    }
}