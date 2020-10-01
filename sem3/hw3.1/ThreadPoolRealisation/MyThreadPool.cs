using System;
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
                        for (var i = 0; i < taskSubmitQueue.Count; i++)
                        {
                            taskSubmitQueue.Dequeue().Invoke();
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
        private readonly CountdownEvent countdownEvent;
        private readonly SynchronizedQueue<Action> tasksQueue = new SynchronizedQueue<Action>();

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }

            var threads = new Thread[threadsCount];
            countdownEvent = new CountdownEvent(threads.Length);
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

            return task;
        }

        private void Submit<TResult>(MyTask<TResult> task)
        {
            tasksQueue.Enqueue(task.Run);
        }

        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested) return;
            cancellationTokenSource.Cancel();
            countdownEvent.Wait();
        }

        private void ExecuteTasks(CancellationToken cancellationToken)
        {
            while (true)
            {
                tasksQueue.Dequeue().Invoke();
                // if (cancellationToken.IsCancellationRequested && tasksQueue.IsEmpty)
                // {
                //     countdownEvent.Signal();
                //     break;
                // }
                //
                // if (cancellationToken.IsCancellationRequested)
                // {
                //     Action taskRunAction;
                //     lock (tasksQueue)
                //     {
                //         if (tasksQueue.IsEmpty)
                //         {
                //             countdownEvent.Signal();
                //             break;
                //         }
                //
                //         tasksQueue.TryDequeue(out taskRunAction);
                //     }
                //
                //     taskRunAction?.Invoke();
                // }
                // else
                // {
                //     if (tasksQueue.TryDequeue(out var taskRunAction))
                //     {
                //         taskRunAction();
                //     }
                //     
                //     semaphore.WaitOne();
                // }
            }
        }
    }
}