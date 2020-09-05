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
            var elapsedTime = TimeGauge.CalculateElapsedTime(x => ++x, 1, out var result);
            
            Assert.That(elapsedTime != TimeSpan.Zero);
            Assert.That(result == 2);
        }
    }
}