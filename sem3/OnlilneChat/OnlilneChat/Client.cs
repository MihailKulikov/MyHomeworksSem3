using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChat
{
    public class Client : IDisposable
    {
        private readonly StreamHandler streamHandler;
        public Client(StreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

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

        public void Dispose()
        {
            streamHandler.Dispose();
        }
    }
}