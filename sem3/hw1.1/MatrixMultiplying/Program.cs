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

                var (myMultipleThreadElapsedTime, _) =
                    TimeGauge.CalculateElapsedTime(firstMatrix.MyVersionOfParallelMultiplyWith, secondMatrix);
                var (multipleThreadElapsedTime, _) = TimeGauge.CalculateElapsedTime(firstMatrix.ParallelForMultiplyWith,
                    secondMatrix);

                Console.WriteLine($"{i}*{i} matrices: {100 * multipleThreadElapsedTime.TotalMilliseconds / myMultipleThreadElapsedTime.TotalMilliseconds}");
            }
        }
    }
}