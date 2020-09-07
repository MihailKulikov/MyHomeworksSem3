using System;
using MatrixMultiplying;
using NUnit.Framework;

namespace MatrixTests
{
    public class TimeGaugeShould
    {
        [Test]
        public void Calculate_ElapsedTime()
        {
            var (elapsed, result) = TimeGauge.CalculateElapsedTime(x => ++x, 1);
            
            Assert.That(elapsed != TimeSpan.Zero);
            Assert.That(result == 2);
        }
    }
}