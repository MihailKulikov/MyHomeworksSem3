using System;
using System.Threading;

namespace LazyInitialization
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isValueCreated;
        private readonly Func<T> supplier;
        private T value;

        public ThreadSafeLazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }
        
        public T Get()
        {
            if (isValueCreated) return value;
            using var mutex = new Mutex();
            mutex.WaitOne();
            if (!isValueCreated)
            {
                value = supplier.Invoke();
                isValueCreated = true;
            }
            mutex.ReleaseMutex();

            return value;
        }
    }
}