using System;

namespace LazyInitialization
{
    /// <summary>
    /// Provides support for thread safe lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isValueCreated;
        private Func<T> supplier;
        private T value;
        private readonly object isValueCreatedLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeLazy{T}"/> class. When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="supplier">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> is null.</exception>
        public ThreadSafeLazy(Func<T> supplier)
        {
            this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        }
        
        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="ThreadSafeLazy{T}"/> instance.
        /// </summary>
        /// <returns>The lazily initialized value of the current <see cref="ThreadSafeLazy{T}"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The initialization function tries to call method Get of this instance.</exception>
        public T Get()
        {
            if (isValueCreated) return value;
            lock (isValueCreatedLock)
            {
                if (isValueCreated) return value;
                var localSupplier = supplier;
                if (localSupplier == null)
                {
                    throw new InvalidOperationException("Recursive calls Get().");
                }

                supplier = null;
                value = localSupplier();
                isValueCreated = true;
            }

            return value;
        }
    }
}