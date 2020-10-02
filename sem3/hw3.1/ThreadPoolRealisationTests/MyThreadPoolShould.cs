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
            threadPool = new MyThreadPool(2);
            
            Assert.That(() => threadPool.Submit<It.IsAnyType>(null), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Return_Result_Of_Calculation_When_Getting_Result_Property_Of_Submitted_Task()
        {
            threadPool = new MyThreadPool(2);

            var task = threadPool.Submit(() => 4 * 4);
            var actualResult = task.Result;
            
            Assert.That(actualResult, Is.EqualTo(16));
            Assert.That(task.IsCompleted, Is.True);
        }

        [Test]
        public void Return_Result_Of_Calculation_When_Getting_Result_Property_Of_Submitted_Task_Twice()
        {
            threadPool = new MyThreadPool(2);

            var task = threadPool.Submit(() =>
            {
                Interlocked.Increment(ref callsCount);
                return 4 * 2;
            });
            var firstActualResult = task.Result;
            var secondActualResult = task.Result;

            Assert.That(firstActualResult, Is.EqualTo(secondActualResult));
            Assert.That(firstActualResult, Is.EqualTo(8));
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(callsCount, Is.EqualTo(1));
        }

        [Test]
        public void Have_Specified_Threads_Count()
        {
            const int threadsCount = 8;
            const int tasksCount = threadsCount + 1;
            threadPool = new MyThreadPool(threadsCount);
            var tasks = new IMyTask<int>[tasksCount];
            using var firstCountDownEvent = new CountdownEvent(threadsCount);
            using var secondCountDownEvent = new CountdownEvent(1);
            
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = threadPool.Submit(() =>
                {
                    Interlocked.Increment(ref callsCount);
        
                    firstCountDownEvent.Signal();
                    secondCountDownEvent.Wait();
        
                    return 2 * 2;
                });
            }
        
            firstCountDownEvent.Wait();
            Assert.That(callsCount, Is.EqualTo(threadsCount));
        }

        [Test]
        public void Throw_AggregateException_When_Getting_Result_Property_Of_The_Task_With_Func_That_Throws_Exception()
        {
            const string exceptionMessage = "Exception";
            threadPool = new MyThreadPool(2);
            var exception = new Exception(exceptionMessage);
            
            var task = threadPool.Submit<int>(() => throw exception);
            try
            {
                var actualResult = task.Result;
            }
            catch (Exception e)
            {
                Assert.That(e, Is.TypeOf<AggregateException>());
                Assert.That(((AggregateException)e).InnerExceptions.Count, Is.EqualTo(1));
                Assert.That(((AggregateException)e).InnerException, Is.EqualTo(exception));
            }

            Assert.That(() => task.Result, Throws.Exception.TypeOf<AggregateException>());
        }

        [Test]
        public void Work_Correctly_With_ContinueWith_Calling()
        {
            threadPool = new MyThreadPool(3);

            var firstTask = threadPool.Submit(() => 2 * 2);
            var secondTask = firstTask.ContinueWith(x => x.ToString());
            
            Assert.That(firstTask.Result, Is.EqualTo(4));
            Assert.That(secondTask.Result, Is.EqualTo("4"));
            Assert.That(firstTask.IsCompleted, Is.True);
            Assert.That(secondTask.IsCompleted, Is.True);
        }

        [Test]
        public void Throw_ArgumentNullException_When_Calling_ContinueWith_With_Null()
        {
            threadPool = new MyThreadPool(2);

            var task = threadPool.Submit(() => 2 + 2);
            
            Assert.That(() => task.ContinueWith<int>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_AggregateException_When_Calling_ContinueWith_Of_Task_That_Throws_Exception()
        {
            const string exceptionMessage = "Exception";
            threadPool = new MyThreadPool(2);
            var exception = new Exception(exceptionMessage);


            var task = threadPool.Submit<int>(() => throw exception).ContinueWith(x => x.ToString());
            try
            {
                var result = task.Result;
            }
            catch (Exception e)
            {
                var actualException = ((AggregateException)((AggregateException) e).InnerException)?.InnerException;
                Assert.That(e, Is.TypeOf<AggregateException>());
                Assert.That(((AggregateException) e).InnerExceptions.Count, Is.EqualTo(1));
                Assert.That(actualException, Is.EqualTo(exception));
            }
        }

        [Test]
        public void Throw_MyThreadPoolShutdownedException_When_Trying_To_Submit_To_Shutdowned_MyThreadPool()
        {
            const string exceptionMessage = "Thread pool shutdowned.";
            threadPool = new MyThreadPool(2);
            threadPool.Shutdown();

            Assert.That(() => threadPool.Submit(() => 2 * 2),
                Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo(exceptionMessage));
        }

         [Test]
         public void Calculate_Task_That_Already_In_ThreadPool_After_Shutdown()
         {
             threadPool = new MyThreadPool(1);
             using var countdownEvent = new CountdownEvent(1);
             var firstTask = threadPool.Submit(() =>
             {
                 countdownEvent.Wait();
                 Interlocked.Increment(ref callsCount);
         
                 return 2 * 2;
             });
             var secondTask = threadPool.Submit(() =>
             {
                 countdownEvent.Wait();
                 Interlocked.Increment(ref callsCount);
         
                 return 4 * 4;
             });
             countdownEvent.Signal();
             threadPool.Shutdown();

             var firstResult = firstTask.Result;
             var secondResult = secondTask.Result;
             
             Assert.That(callsCount, Is.EqualTo(2));
             Assert.That(firstResult, Is.EqualTo(4));
             Assert.That(secondResult, Is.EqualTo(16));
             Assert.That(firstTask.IsCompleted, Is.True);
             Assert.That(secondTask.IsCompleted, Is.True);
         }

         [Test]
         public void Calculate_ContinueWithTask_That_Already_In_ThreadPool_After_Shutdown()
         {
             threadPool = new MyThreadPool(1);
             using var countdownEvent = new CountdownEvent(1);
             var initialTask = threadPool.Submit(() =>
             {
                 countdownEvent.Wait();
                 Interlocked.Increment(ref callsCount);
         
                 return 2 * 2;
             });
             var continueWithTask = initialTask.ContinueWith(x =>
             {
                 countdownEvent.Wait();
                 Interlocked.Increment(ref callsCount);
         
                 return x.ToString();
             });
             countdownEvent.Signal();
             threadPool.Shutdown();
         
             var firstResult = initialTask.Result;
             var secondResult = continueWithTask.Result;
         
             Assert.That(callsCount, Is.EqualTo(2));
             Assert.That(firstResult, Is.EqualTo(4));
             Assert.That(secondResult, Is.EqualTo("4"));
             Assert.That(initialTask.IsCompleted, Is.True);
             Assert.That(continueWithTask.IsCompleted, Is.True);
         }

         [Test]
         public void Correctly_Shutdown_Without_Tasks()
         {
             threadPool = new MyThreadPool(3);
             
             threadPool.Shutdown();
             
             Assert.Pass();
         }
         
        [Test]
        public void Calculates_Continue_With_Tasks_In_DifferentThreads()
        {
            threadPool = new MyThreadPool(2);
            using var countdownEvent = new CountdownEvent(1);
            var initialTask = threadPool.Submit(() =>
            {
                Interlocked.Increment(ref callsCount);
                
                return 2 * 2;
            });
            
            var result = initialTask.Result;
            var firstContinueWith = initialTask.ContinueWith(x =>
            {
                countdownEvent.Wait();
                Interlocked.Increment(ref callsCount);

                return x.ToString();
            });
            var secondContinueWith = initialTask.ContinueWith(x =>
            {
                Interlocked.Increment(ref callsCount);

                return x + "1";
            });

            Assert.That(secondContinueWith.Result, Is.EqualTo("41"));
            Assert.That(secondContinueWith.IsCompleted, Is.True);
            Assert.That(callsCount, Is.EqualTo(2));
            countdownEvent.Signal();
            Assert.That(firstContinueWith.Result, Is.EqualTo("4"));
            Assert.That(firstContinueWith.IsCompleted, Is.True);
        }

        [Test]
        public void Calculate_Two_Submitted_Tasks_With_One_Thread()
        {
            threadPool = new MyThreadPool(1);
            var firstTask = threadPool.Submit(() => 2 * 2);
            var secondTask = threadPool.Submit(() => 5 * 4);

            Assert.That(firstTask.Result, Is.EqualTo(4));
            Assert.That(firstTask.IsCompleted, Is.True);
            Assert.That(secondTask.Result, Is.EqualTo(20));
            Assert.That(secondTask.IsCompleted, Is.True);
        }

        [Test]
        public void ReturnFalse_After_Getting_IsCompleted_Property_When_Task_Not_Completed()
        {
            threadPool = new MyThreadPool(1);
            using var countdownEvent = new CountdownEvent(1);
            var task = threadPool.Submit(() =>
            {
                countdownEvent.Wait();

                return 2 * 2;
            });

            Assert.That(task.IsCompleted, Is.False);
        }

        [Test]
        public void
            Not_Block_The_Work_Of_The_Thread_When_Calling_ContinueWith_To_The_Task_Which_Result_Has_Not_Been_Calculated_Yet()
        {
            threadPool = new MyThreadPool(2);
            using var countdownEvent = new CountdownEvent(1);
            var task = threadPool.Submit(() =>
            {
                countdownEvent.Wait();

                return 2 * 2;
            }).ContinueWith(x => x.ToString());

            Assert.That(task.IsCompleted, Is.False);
            countdownEvent.Signal();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            
        }
    }
}