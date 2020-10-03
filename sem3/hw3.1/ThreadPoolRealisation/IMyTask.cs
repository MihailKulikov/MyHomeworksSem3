using System;

namespace ThreadPoolRealisation
{
    /// <summary>
    /// Represents an operation that executes in the <see cref="MyThreadPool"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the <see cref="IMyTask{TResult}"/></typeparam>
    public interface IMyTask<out TResult>
    {
        /// <summary>
        /// Gets a value that indicates whether the task has completed.
        /// </summary>
        bool IsCompleted { get; }
        
        /// <summary>
        /// Gets the result value of this <see cref="IMyTask{TResult}"/>.
        /// </summary>
        TResult Result { get; }
        
        /// <summary>
        /// Creates a continuation that executes in the <see cref="MyThreadPool"/> when the target <see cref="IMyTask{TResult}"/> completes.
        /// </summary>
        /// <param name="continuationFunction">A function to run when the <see cref="IMyTask{TResult}"/> completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
        /// <returns>A new continuation <see cref="IMyTask{TResult}"/>.</returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction);
    }
}