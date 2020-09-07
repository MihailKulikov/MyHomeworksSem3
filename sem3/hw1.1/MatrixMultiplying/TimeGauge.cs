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
        /// Calculate elapsed time of specified func invocation.
        /// </summary>
        /// <param name="measurableFunc">The specified func.</param>
        /// <param name="arg">Func argument for invocation.</param>
        /// <typeparam name="TArg">The type of func parameter.</typeparam>
        /// <typeparam name="TRes">The type of the return value of the func.</typeparam>
        /// <returns>Elapsed working time and the result of the func invocation.</returns>
        public static (TimeSpan Elapsed, TRes Result) CalculateElapsedTime<TArg, TRes>(Func<TArg, TRes> measurableFunc, TArg arg)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = measurableFunc.Invoke(arg);
            stopwatch.Stop();

            return (stopwatch.Elapsed, result);
        }
    }
}