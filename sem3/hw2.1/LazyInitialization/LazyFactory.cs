using System;

namespace LazyInitialization
{
    /// <summary>
    /// Provides methods for initializing <see cref="ILazy{T}"/> interface implementations.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Initializes and returns a new instance of the <see cref="CreateThreadSafeLazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <returns>A new instance of the <see cref="CreateThreadSafeLazy{T}"/> class.</returns>
        public static ILazy<T> CreateThreadSafeLazy<T>(Func<T> supplier)
            => new ThreadSafeLazy<T>(supplier);

        /// <summary>
        /// Initializes and returns a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <returns>A new instance of the <see cref="Lazy{T}"/> class.</returns>
        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
            => new Lazy<T>(supplier);
    }
}