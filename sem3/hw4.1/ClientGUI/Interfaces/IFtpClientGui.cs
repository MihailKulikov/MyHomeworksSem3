using System;
using System.Threading.Tasks;
using Client.Interfaces;

namespace ClientGUI.Interfaces
{
    public interface IFtpClientGui : IFtpClient
    {
        public Task<string> GetAsync(string path, Action<int> updateProgressPercentageAction);
    }
}