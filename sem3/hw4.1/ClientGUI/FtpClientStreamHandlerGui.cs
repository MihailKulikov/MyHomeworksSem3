using System;
using System.IO;
using System.Threading.Tasks;
using Client;
using ClientGUI.Interfaces;

namespace ClientGUI
{
    public class FtpClientStreamHandlerGui :  FtpClientStreamHandler, IFtpClientStreamHandlerGui
    {
        private const int BufferSize = 4096;
        
        public FtpClientStreamHandlerGui(Stream stream) : base(stream)
        {
        }

        public async Task CopyToAsync(Stream destination, long count, Action<int> updatePercentageAction)
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
                updatePercentageAction((int)percentage);
            }

            await WriteFromStreamToStreamUsingBuffer(intermediateStorage, (int) count % intermediateStorage.Length,
                destination);
            updatePercentageAction(100);
        }
    }
}