using System;
using System.Diagnostics;

namespace MatrixMultiplying
{
    /// <summary>
    /// Represents time gauge. Provides methods to calculate elapsed time of specified func.
    /// </summary>
    public static class TimeGauge
    {
        /// <summary>
        /// Calculate elapsed time of specified func.
        /// </summary>
        /// <param name="measurableFunc">The specified func.</param>
        /// <param name="arg">Func argument for invocation.</param>
        /// <param name="result">The result of the func invocation.</param>
        /// <typeparam name="TArg">The type of func parameter.</typeparam>
        /// <typeparam name="TRes">The type of the return value of the func.</typeparam>
        /// <returns></returns>
        public static TimeSpan CalculateElapsedTime<TArg, TRes>(Func<TArg, TRes> measurableFunc, TArg arg, out TRes result)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            result = measurableFunc.Invoke(arg);
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}