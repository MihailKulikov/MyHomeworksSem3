using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPoolRealisation
{
    /// <summary>
    /// Provides a pool of threads that can be used to execute tasks.
    /// </summary>
    public sealed class MyThreadPool : IDisposable
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private Func<TResult> func;
            private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            private TResult result;
            private AggregateException aggregateException;
            private readonly Queue<Action> queueOfContinueWithTasks = new Queue<Action>();
            private readonly MyThreadPool threadPool;
            private volatile bool isCompleted;

            public MyTask(Func<TResult> func, MyThreadPool threadPool)
            {
                this.threadPool = threadPool;
                this.func = func;
            }

            public bool IsCompleted
            {
                get => isCompleted;
                private set => isCompleted = value;
            }

            public TResult Result
            {
                get
                {
                    manualResetEvent.WaitOne();
                    return GetResultOrThrowException();
                }
                private set => result = value;
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction)
            {
                if (continuationFunction == null)
                {
                    throw new ArgumentNullException(nameof(continuationFunction));
                }

                if (threadPool.cancellationTokenSource.IsCancellationRequested)
                {
                    throw new InvalidOperationException("Thread pool shutdowned.");
                }

                var task = new MyTask<TNewResult>(() => continuationFunction(Result), threadPool);
                if (IsCompleted)
                {
                    threadPool.Submit(task);
                }
                else
                {
                    lock (queueOfContinueWithTasks)
                    {
                        if (IsCompleted)
                        {
                            threadPool.Submit(task);
                        }
                        else
                        {
                            queueOfContinueWithTasks.Enqueue(() => threadPool.Submit(task));
                        }
                    }
                }

                return task;
            }

            public void Run()
            {
                try
                {
                    Result = func();
                }
                catch (Exception e)
                {
                    aggregateException = new AggregateException(e);
                }
                finally
                {
                    lock (queueOfContinueWithTasks)
                    {
                        while (queueOfContinueWithTasks.Count != 0)
                        {
                            queueOfContinueWithTasks.Dequeue().Invoke();
                        }    

                        IsCompleted = true;
                    }

                    manualResetEvent.Set();
                    func = null;
                }
            }

            private TResult GetResultOrThrowException()
            {
                if (aggregateException != null)
                {
                    throw aggregateException;
                }

                return result;
            }

            public void Dispose()
            {
                manualResetEvent.Dispose();
                threadPool.Dispose();
            }
        }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CountdownEvent countdownEventForShutdown;
        private readonly BlockingCollection<Action> collectionOfPendingTasks = new BlockingCollection<Action>();

        /// <summary>
        /// Initialize a <see cref="MyThreadPool"/> instance with the specified number of threads.
        /// </summary>
        /// <param name="numberOfThreads">Specified number of threads.</param>
        /// <exception cref="ArgumentException"><paramref name="numberOfThreads"/> is not positive.</exception>
        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }

            var threads = new Thread[numberOfThreads];
            countdownEventForShutdown = new CountdownEvent(threads.Length);
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() => ExecuteTasks(cancellationTokenSource.Token));
                threads[i].Start();
            }
        }

        /// <summary>
        /// Creates and starts a task of type TResult for the specified function delegate.
        /// </summary>
        /// <param name="func">A function delegate that returns the future result to be available through the task.</param>
        /// <typeparam name="TResult">The type of the result available through the task.</typeparam>
        /// <returns>The started task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><see cref="MyThreadPool"/> shutdowned.</exception>
        public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Thread pool shutdowned.");
            }

            var task = new MyTask<TResult>(func, this);
            collectionOfPendingTasks.Add(task.Run);

            return task;
        }

        private void Submit<TResult>(MyTask<TResult> task)
        {
            collectionOfPendingTasks.Add(task.Run);
        }

        /// <summary>
        /// Terminate threads from the <see cref="MyThreadPool"/>.
        /// Already running tasks are not interrupted, but new tasks are not accepted for execution by threads from the pool.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested) return;
            cancellationTokenSource.Cancel();
            countdownEventForShutdown.Wait();
        }

        private void ExecuteTasks(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    if (collectionOfPendingTasks.TryTake(out var taskRunAction))
                    {
                        taskRunAction.Invoke();
                    }
                    else
                    {
                        countdownEventForShutdown.Signal();
                        break;
                    }
                }
                else
                {
                    try
                    {
                        var taskRunAction = collectionOfPendingTasks.Take(cancellationToken);

                        taskRunAction.Invoke();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Release all resources used by the current instance of the <see cref="MyThreadPool"/> class.
        /// </summary>
        public void Dispose()
        {
            Shutdown();
            cancellationTokenSource.Dispose();
            countdownEventForShutdown.Dispose();
            collectionOfPendingTasks.Dispose();
        }
    }
}