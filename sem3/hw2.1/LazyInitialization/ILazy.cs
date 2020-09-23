namespace LazyInitialization
{
    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    public interface ILazy<out T>
    {
        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="ILazy{T}"/> instance.
        /// </summary>
        /// <returns>The lazily initialized value of the current <see cref="ILazy{T}"/> instance.</returns>
        T Get();
    }
}