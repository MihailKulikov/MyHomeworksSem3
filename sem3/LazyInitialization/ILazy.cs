namespace LazyInitialization
{
    public interface ILazy<out T>
    {
        T Get();
    }
}