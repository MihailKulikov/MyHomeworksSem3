using System;
using System.Threading;
using NUnit.Framework;
using ThreadPoolRealisation;
using Moq;

namespace ThreadPoolRealisationTests
{
    public class MyThreadPoolShould
    {
        private MyThreadPool threadPool;
        private volatile int callsCount;
        private CountdownEvent countdownEvent;

        [SetUp]
        public void Setup()
        {   
            countdownEvent = new CountdownEvent(1);
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
                Interlocked.Increment(ref callsCount);
                return 4 * 2;
            });
            var firstActualResult = task.Result;
            var secondActualResult = task.Result;

            Assert.That(firstActualResult, Is.EqualTo(secondActualResult));
            Assert.That(firstActualResult, Is.EqualTo(8));
            Assert.That(task.IsCompleted);
            Assert.That(callsCount, Is.EqualTo(1));
        }

        // [Test]
        // public void Have_Specified_Threads_Count()
        // {
        //     const int threadsCount = 8;
        //     const int tasksCount = threadsCount + 1;
        //     threadPool = new MyThreadPool(threadsCount);
        //     var tasks = new IMyTask<int>[tasksCount];
        //     using var localCountDownEvent = new CountdownEvent(threadsCount);
        //     
        //     for (var i = 0; i < tasks.Length; i++)
        //     {
        //         tasks[i] = threadPool.Submit(() =>
        //         {
        //             Interlocked.Increment(ref callsCount);
        //
        //             localCountDownEvent.Signal();
        //             countdownEvent.Wait();
        //
        //             return 2 * 2;
        //         });
        //     }
        //
        //     localCountDownEvent.Wait();
        //     Assert.That(callsCount, Is.EqualTo(threadsCount));
        // }

        [Test]
        public void Throw_AggregateException_When_Getting_Result_Property_Of_The_Task_With_Func_That_Throws_Exception()
        {
            const string exceptionMessage = "Exception";
            threadPool = new MyThreadPool(1);
            var exception = new Exception(exceptionMessage);
            
            try
            {
                var task = threadPool.Submit<int>(() => throw exception);
                var actualResult = task.Result;
            }
            catch (AggregateException e)
            {
                Assert.That(e, Is.TypeOf<AggregateException>());
                Assert.That(e.InnerExceptions.Count, Is.EqualTo(1));
                Assert.That(e.InnerException, Is.EqualTo(exception));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            countdownEvent.Dispose();
        }
    }
}