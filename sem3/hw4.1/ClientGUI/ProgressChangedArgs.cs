using System;

namespace ClientGUI
{
    public class ProgressChangedArgs : EventArgs
    {
        public ProgressChangedArgs(int newProgress)
        {
            Progress = newProgress > 100 ? Math.Min(100, newProgress) : Math.Max(0, newProgress);
        }   
        
        public int Progress { get; set; }
    }
}