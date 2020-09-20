using System;
using System.Threading;

namespace LazyInitialization
{
    /// <summary>
    /// Provides support for thread safe lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isValueCreated;
        private readonly Func<T> supplier;
        private T value;
        private readonly Mutex mutex = new Mutex();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeLazy{T}"/> class. When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="supplier">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        public ThreadSafeLazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }
        
        public T Get()
        {
            if (isValueCreated) return value;
            try
            {
                mutex.WaitOne();
                if (!isValueCreated)
                {
                    value = supplier.Invoke();
                    isValueCreated = true;
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            return value;
        }
        
        ~ThreadSafeLazy()
        {
            mutex.Dispose();
        }
    }
}