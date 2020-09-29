using NUnit.Framework;
using ThreadPoolRealisation;
using Moq;

namespace ThreadPoolRealisationTests
{
    public class MyThreadPoolShould
    {
        private MyThreadPool threadPool;
        private int callsCount;

        [SetUp]
        public void Setup()
        {
            callsCount = 0;
        }

        [TestCase(-1)]
        [TestCase(0)]
        [Test]
        public void Throw_ArgumentException_When_Threads_Is_Not_Positive(int threadsCount)
        {
            const string exceptionMessage = "Threads' count is not positive.";
            
            Assert.That(() => new MyThreadPool(threadsCount),
                Throws.ArgumentException.And.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void Throw_ArgumentNullException_When_Calling_Submit_With_Null_Func()
        {
            threadPool = new MyThreadPool(1);
            
            Assert.That(() => threadPool.Submit<It.IsAnyType>(null), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Return_Result_Of_Calculation_When_Getting_Result_Property_Of_Submitted_Task()
        {
            threadPool = new MyThreadPool(1);

            var task = threadPool.Submit(() => 4 * 4);
            var actualResult = task.Result;
            
            Assert.That(actualResult, Is.EqualTo(16));
            Assert.That(task.IsCompleted);
        }

        [Test]
        public void Return_Result_Of_Calculation_When_Getting_Result_Property_Of_Submitted_Task_Twice()
        {
            threadPool = new MyThreadPool(1);

            var task = threadPool.Submit(() =>
            {
                callsCount++;
                return 4 * 2;
            });
            var firstActualResult = task.Result;
            var secondActualResult = task.Result;

            Assert.That(firstActualResult, Is.EqualTo(secondActualResult));
            Assert.That(firstActualResult, Is.EqualTo(8));
            Assert.That(task.IsCompleted);
            Assert.That(callsCount, Is.EqualTo(1));
        }
    }
}