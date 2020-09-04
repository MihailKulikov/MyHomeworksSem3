using System;
using System.Diagnostics;

namespace MatrixMultiplying
{
    public static class TimeGauge
    {
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