using System;

namespace LazyInitialization
{
    /// <summary>
    /// Provides support for not thread safe lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private bool isValueCreated;
        private readonly Func<T> supplier;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class. When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="supplier">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        public Lazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }
        
        public T Get()
        {
            if (isValueCreated) return value;
            value = supplier.Invoke();
            isValueCreated = true;

            return value;
        }
    }
}