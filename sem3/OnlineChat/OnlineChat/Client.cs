using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChat
{
    /// <summary>
    /// Represents client for online chat.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly StreamHandler streamHandler;
        
        /// <summary>
        /// Initialize new instance of the <see cref="Client"/> class with specified instance of the <see cref="StreamHandler"/> class.
        /// </summary>
        /// <param name="streamHandler">Specified instance of the <see cref="StreamHandler"/> class.</param>
        public Client(StreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

        /// <summary>
        /// Starts the client for writing and reading.
        /// </summary>
        public void Start()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            var writeToStreamTask = Task.Run(async () => await streamHandler.WriteToStream(),
                cancellationTokenSource.Token);
            var readFromStreamTask = Task.Run(async () => await streamHandler.ReadFromStream(),
                cancellationTokenSource.Token);
            Task.WaitAny(writeToStreamTask, readFromStreamTask);
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Releases all resources used by <see cref="Client"/> object.
        /// </summary>
        public void Dispose()
        {
            streamHandler.Dispose();
        }
    }
}