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
                        var taskSubmitCount = queueOfContinueWithTasks.Count;
                        for (var i = 0; i < taskSubmitCount; i++)
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

            ~MyTask()
            {
                manualResetEvent.Dispose();
            }
        }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CountdownEvent countdownEventForShutdown;
        private readonly BlockingCollection<Action> collectionOfPendingTasks = new BlockingCollection<Action>();

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }

            var threads = new Thread[threadsCount];
            countdownEventForShutdown = new CountdownEvent(threads.Length);
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
                throw new InvalidOperationException("Thread pool shutdowned.");
            }

            var task = new MyTask<TResult>(func, this);
            collectionOfPendingTasks.Add(task.Run);

            return task;
        }

        private void Submit<TResult>(MyTask<TResult> task)
        {
            try
            {
                collectionOfPendingTasks.Add(task.Run);
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException("Thread pool shutdowned.");
            }
        }

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
    }
}