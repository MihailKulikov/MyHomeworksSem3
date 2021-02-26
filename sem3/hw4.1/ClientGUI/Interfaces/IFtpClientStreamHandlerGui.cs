using System;
using System.IO;
using System.Threading.Tasks;
using Client.Interfaces;

namespace ClientGUI.Interfaces
{
    public interface IFtpClientStreamHandlerGui : IFtpClientStreamHandler
    {
        public event EventHandler<ProgressChangedArgs> RaiseProgressChangedEvent;
    }
}