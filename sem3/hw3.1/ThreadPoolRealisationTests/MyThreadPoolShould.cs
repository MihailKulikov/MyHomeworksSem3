using NUnit.Framework;
using ThreadPoolRealisation;
using Moq;

namespace ThreadPoolRealisationTests
{
    public class MyThreadPoolShould
    {
        private MyThreadPool threadPool;
        
        [SetUp]
        public void Setup()
        {
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
    }
}