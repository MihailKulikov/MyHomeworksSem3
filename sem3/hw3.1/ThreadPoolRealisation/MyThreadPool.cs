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
            private TResult result;
            private Func<TResult> func;
            
            public MyTask(Func<TResult> func)
            {
                this.func = func;
            }
            
            
            
            public bool IsCompleted { get; }
            public TResult Result => result;
            public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                throw new NotImplementedException();
            }
        }

        private readonly List<Thread> threads;
        private ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentException("Threads' count is not positive.");
            }
            
            threads = new List<Thread>();
            for (var i = 0; i < threads.Count; i++)
            {
                threads[i] = new Thread(ExecuteTasks);
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            throw new NotImplementedException();
        }
        
        private static void ExecuteTasks()
        {
            throw new NotImplementedException();
        }
    }
}