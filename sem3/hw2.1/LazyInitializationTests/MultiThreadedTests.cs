using System.Threading;
using LazyInitialization;
using NUnit.Framework;

namespace LazyInitializationTests
{
    public class MultiThreadedTests
    {
        private volatile int callsCount;
        private ThreadSafeLazy<object> threadSafeLazy;
        private readonly object expectedObject = new object();
        private readonly CountdownEvent countdownEvent = new CountdownEvent(1);

        [SetUp]
        public void SetUp()
        {
            callsCount = 0;
        }   
        
        [Test]
        public void ThreadSafeLazy_Should_Get_Value_Concurrent_In_Different_Threads_Without_Races()
        {
            const int threadCount = 10;
            threadSafeLazy = new ThreadSafeLazy<object>(() =>
            {
                Interlocked.Increment(ref callsCount);
                
                return expectedObject;
            });
            var threads = new Thread[threadCount];
            var actualObjects = new object[threadCount];
            
            for (var i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    countdownEvent.Wait();
                    actualObjects[localI] = threadSafeLazy.Get();
                });
                
                threads[i].Start();
            }
            countdownEvent.Signal();
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            Assert.That(callsCount, Is.EqualTo(1));
            foreach (var actualObject in actualObjects)
            {
                Assert.That(actualObject, Is.EqualTo(expectedObject));
            }
        }

        [TearDown]
        public void TearDown()
        {
            countdownEvent.Dispose();
        }
    }
}