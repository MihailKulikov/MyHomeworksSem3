using System;
using System.Collections.Concurrent;
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

            public MyTask(Func<TResult> func)
            {
                this.func = func;
            }
            
            public bool IsCompleted { get; private set; }

            public TResult Result
            {
                get
                {
                    if (IsCompleted)
                    {
                        return result;
                    }

                    lock (isCompletedLock)
                    {
                        if (IsCompleted)
                        {
                            return result;
                        }

                        semaphore.WaitOne();
                        return result;
                    }
                }
                private set => result = value;
            }

            public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                throw new NotImplementedException();
            }

            public void Run()
            {
                Result = func();
                IsCompleted = true;
                semaphore.Release();
            }
        }

        private readonly ConcurrentQueue<Action> tasksQueue = new ConcurrentQueue<Action>();
        private readonly Semaphore semaphore = new Semaphore(0, int.MaxValue);

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }
            
            var threads = new Thread[threadsCount];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ExecuteTasks);
                threads[i].Start();
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            
            var task = new MyTask<TResult>(func);
            tasksQueue.Enqueue(() => task.Run());
            semaphore.Release();

            return task;
        }
        
        private void ExecuteTasks()
        {
            while (true)
            {
                semaphore.WaitOne();

                if (tasksQueue.TryDequeue(out var taskRunAction))
                {
                    taskRunAction();
                }
            }
        }
    }
}