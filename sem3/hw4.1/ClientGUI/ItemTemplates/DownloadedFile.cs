using System.ComponentModel;

namespace ClientGUI.ItemTemplates
{
    public class DownloadedFile : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int completion;
        public int Completion
        {
            get => completion;
            set
            {
                if (completion == value) return;
                completion = value;
                NotifyPropertyChanged(nameof(Completion));
            } }
        
        public DownloadedFile(string name, int completion)
        {
            Name = name;
            Completion = completion;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}