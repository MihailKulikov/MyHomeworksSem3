using System;
using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework;
using ThreadPoolRealisation;

namespace ThreadPoolRealisationTests
{
    public class TestThatChecksThreadSafety
    {
        private MyThreadPool threadPool;
        private volatile int callsCount;

        [SetUp]
        public void SetUp()
        {
            callsCount = 0;
        }

        [Test]
        public void ContinueWith_Should_Be_ThreadSafe_Then_Calling_To_One_Task()
        {
            const int threadCount = 10;
            var threads = new Thread[threadCount];
            var tasks = new ConcurrentBag<IMyTask<string>>();
            threadPool = new MyThreadPool(Environment.ProcessorCount);
            using var countdownEvent = new CountdownEvent(1);
            var initialTask = threadPool.Submit(() =>
            {
                countdownEvent.Wait();
                Thread.Sleep(10);
                return 2 * 2;
            });
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    countdownEvent.Wait();

                    tasks.Add(initialTask.ContinueWith(x => x + "2"));
                });

                threads[i].Start();
            }
            countdownEvent.Signal();
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(initialTask.Result, Is.EqualTo(4));
            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.EqualTo("42"));
            }
        }

        [Test]
        public void ResultProperty_Should_Be_ThreadSafe_If_Func_Is_Correct()
        {
            const int threadCount = 10;
            var threads = new Thread[threadCount];
            var results = new int[threadCount];
            threadPool = new MyThreadPool(Environment.ProcessorCount);
            using var countdownEvent = new CountdownEvent(1);
            var task = threadPool.Submit(() =>
            {
                Interlocked.Increment(ref callsCount);

                return 2 * 2;
            });
            for (var i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    countdownEvent.Wait();

                    results[localI] = task.Result;
                });
                threads[i].Start();
            }

            countdownEvent.Signal();
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(callsCount, Is.EqualTo(1));
            foreach (var result in results)
            {
                Assert.That(result, Is.EqualTo(4));
            }
        }

        [Test]
        public void ResultProperty_Should_Be_ThreadSafe_If_Func_Throws_Exception()
        {
            const int threadCount = 10;
            const string exceptionMessage = "exception";
            var thrownException = new Exception(exceptionMessage);
            var threads = new Thread[threadCount];
            var catchedExceptions = new AggregateException[threadCount];
            threadPool = new MyThreadPool(Environment.ProcessorCount);
            using var countdownEvent = new CountdownEvent(1);
            var task = threadPool.Submit<int>(() =>
            {
                Interlocked.Increment(ref callsCount);

                throw thrownException;
            });
            for (var i = 0; i < threads.Length; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    countdownEvent.Wait();

                    try
                    {
                        var result = task.Result;
                    }
                    catch (Exception e)
                    {
                        catchedExceptions[localI] = (AggregateException) e;
                    }
                });
                threads[i].Start();
            }

            countdownEvent.Signal();
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(callsCount, Is.EqualTo(1));
            foreach (var exception in catchedExceptions)
            {
                Assert.That(exception, Is.TypeOf<AggregateException>());
                Assert.That(exception.InnerExceptions.Count, Is.EqualTo(1));
                Assert.That(exception.InnerException, Is.EqualTo(thrownException));
            }
        }

        [Test]
        public void Submit_Should_Be_ThreadSafe()
        {
            const int threadCount = 10;
            var threads = new Thread[threadCount];
            var tasks = new ConcurrentBag<IMyTask<int>>();
            threadPool = new MyThreadPool(Environment.ProcessorCount);
            using var countdownEvent = new CountdownEvent(1);

            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    countdownEvent.Wait();
                    tasks.Add( threadPool.Submit(() =>
                    {
                        Interlocked.Increment(ref callsCount);

                        return 2 * 2;
                    }));
                });
                threads[i].Start();
            }

            countdownEvent.Signal();
            foreach (var thread in threads)
            {
                thread.Join();
            }

            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.EqualTo(4));
            }

            Assert.That(callsCount, Is.EqualTo(threadCount));
        }
    }
}