using System;
using System.Threading.Tasks;

namespace ThreadPoolRealisation
{
    public interface IMyTask<out TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
        TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
        Task<>
    }
}