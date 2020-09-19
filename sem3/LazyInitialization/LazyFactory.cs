using System;

namespace LazyInitialization
{
    public static class LazyFactory
    {
        public static ILazy<T> CreateThreadSafeLazy<T>(Func<T> supplier)
            => new ThreadSafeLazy<T>(supplier);

        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
            => new Lazy<T>(supplier);
    }
}