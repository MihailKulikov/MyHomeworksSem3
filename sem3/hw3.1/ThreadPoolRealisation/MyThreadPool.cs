using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolRealisation
{
    public class MyThreadPool
    {
        private readonly List<Thread> threads;
        private ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();

        public MyThreadPool(int threadsCount)
        {
            threads = new List<Thread>();
            for (var i = 0; i < threads.Count; i++)
            {
                threads[i] = new Thread(ExecuteTasks);
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
        {
            throw new NotImplementedException();
        }
        
        private static void ExecuteTasks()
        {
            throw new NotImplementedException();
        }
    }
}