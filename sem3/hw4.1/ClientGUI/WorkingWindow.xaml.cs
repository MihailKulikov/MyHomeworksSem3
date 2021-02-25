using System.Linq;
using ClientGUI.ItemTemplates;
using System.Windows.Input;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for WorkingWindow.xaml
    /// </summary>
    public partial class WorkingWindow
    {
        public WorkingWindow()
        {
            InitializeComponent();
        }

        public WorkingWindow(ViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private void GoToCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DirectoryList.SelectedItem as ListResult)?.IsDirectory ?? false;
        }

        private void GoToCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as ViewModel)?.GoTo((DirectoryList.SelectedItem as ListResult)?.Name ?? "./");
        }

        private void DownloadCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(DirectoryList.SelectedItem as ListResult)?.IsDirectory ?? false;
        }

        private void DownloadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DirectoryList.SelectedItem is ListResult file)
            {
                (DataContext as ViewModel)?.Download(file.Name);
            }
        }

        private void DownloadAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as ViewModel)?.DownloadAll();
        }

        private void DownloadAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DataContext as ViewModel)?.ListResults.FirstOrDefault(result => !result.IsDirectory) != null;
        }
    }
}
