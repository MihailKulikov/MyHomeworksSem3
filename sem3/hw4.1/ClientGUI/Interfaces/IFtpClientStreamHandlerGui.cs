using System;
using Client.Interfaces;

namespace ClientGUI.Interfaces
{
    public interface IFtpClientStreamHandlerGui : IFtpClientStreamHandler
    {
        public event EventHandler<ProgressChangedArgs> RaiseProgressChangedEvent;
    }
}