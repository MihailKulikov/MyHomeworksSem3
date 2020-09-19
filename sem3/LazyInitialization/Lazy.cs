using System;

namespace LazyInitialization
{
    public class Lazy<T> : ILazy<T>
    {
        private bool isValueCreated;
        private readonly Func<T> supplier;
        private T value;

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