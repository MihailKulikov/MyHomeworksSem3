using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPoolRealisation
{
    public class MyThreadPool
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private readonly Func<TResult> func;
            private readonly Semaphore semaphore = new Semaphore(0, 1);
            private TResult result;
            private readonly object isCompletedLock = new object();
            private AggregateException aggregateException;
            private readonly Queue<Action> taskSubmitQueue = new Queue<Action>();
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
                    if (IsCompleted)
                    {
                        return GetResultOrThrowException();
                    }

                    lock (isCompletedLock)
                    {
                        if (IsCompleted)
                        {
                            GetResultOrThrowException();
                        }

                        semaphore.WaitOne();
                        return GetResultOrThrowException();
                    }
                }
                private set => result = value;
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueWithFunc)
            {
                if (continueWithFunc == null)
                {
                    throw new ArgumentNullException(nameof(continueWithFunc));
                }

                var task = new MyTask<TNewResult>(() => continueWithFunc(Result), threadPool);
                if (IsCompleted)
                {
                    threadPool.Submit(task);
                }
                else
                {
                    lock (taskSubmitQueue)
                    {
                        if (IsCompleted)
                        {
                            threadPool.Submit(task);
                        }
                        else
                        {
                            taskSubmitQueue.Enqueue(() => threadPool.Submit(task));
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
                    lock (taskSubmitQueue)
                    {
                        foreach (var taskSubmitAction in taskSubmitQueue)
                        {
                            taskSubmitAction();
                        }

                        IsCompleted = true;
                    }

                    semaphore.Release();
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

            ~MyTask()
            {
                semaphore.Dispose();
            }
        }

        private readonly ConcurrentQueue<Action> tasksQueue = new ConcurrentQueue<Action>();
        private readonly Semaphore semaphore = new Semaphore(0, int.MaxValue);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }

            var threads = new Thread[threadsCount];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() => ExecuteTasks(cancellationTokenSource.Token));
                threads[i].Start();
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new MyThreadPoolShutdownedException("Thread pool shutdowned.");
            }

            var task = new MyTask<TResult>(func, this);
            tasksQueue.Enqueue(task.Run);
            semaphore.Release();

            return task;
        }

        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested) return;
            cancellationTokenSource.Cancel();
        }

        private void Submit<TResult>(MyTask<TResult> task)
        {
            tasksQueue.Enqueue(task.Run);
            semaphore.Release();
        }

        private void ExecuteTasks(CancellationToken cancellationToken)
        {
            //TODO: correct work after shutdown
            while (true)
            {
                if (cancellationToken.IsCancellationRequested && tasksQueue.IsEmpty)
                {
                    break;
                }

                semaphore.WaitOne();

                if (tasksQueue.TryDequeue(out var taskRunAction))
                {
                    taskRunAction();
                }
            }
        }
    }
}