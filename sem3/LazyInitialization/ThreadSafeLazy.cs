using System;
using System.Threading;

namespace LazyInitialization
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isValueCreated;
        private readonly Mutex mutex = new Mutex();
        private readonly Func<T> supplier;
        private T value;

        public ThreadSafeLazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }
        
        public T Get()
        {
            if (isValueCreated) return value;
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