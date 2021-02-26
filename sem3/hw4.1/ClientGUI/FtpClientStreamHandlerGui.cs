using System;
using System.IO;
using System.Threading.Tasks;
using Client;
using ClientGUI.Interfaces;

namespace ClientGUI
{
    public sealed class FtpClientStreamHandlerGui :  FtpClientStreamHandler, IFtpClientStreamHandlerGui
    {
        private const int BufferSize = 4096;
        
        public FtpClientStreamHandlerGui(Stream stream) : base(stream)
        {
        }

        public override async Task CopyToAsync(Stream destination, long count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count should be not negative.");
            }

            var percentage = 0d;
            var intermediateStorage = new byte[BufferSize];
            for (var i = 0; i < count / BufferSize; i++)
            {
                await WriteFromStreamToStreamUsingBuffer(intermediateStorage, intermediateStorage.Length, destination);
                percentage +=  BufferSize * 100.0 / count;
                OnRaiseProgressChangedEvent(new ProgressChangedArgs((int) percentage));
            }

            await WriteFromStreamToStreamUsingBuffer(intermediateStorage, (int) count % intermediateStorage.Length,
                destination);
            OnRaiseProgressChangedEvent(new ProgressChangedArgs(100));
        }

        private void OnRaiseProgressChangedEvent(ProgressChangedArgs e)
        {
            RaiseProgressChangedEvent(this, e);
        }

        public event EventHandler<ProgressChangedArgs> RaiseProgressChangedEvent = (sender, args) => { };
    }
}