using ClientGUI.ItemTemplates;
using System.Windows;
using System.Windows.Input;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(ViewModel viewModel) : this()
        {
            DataContext = viewModel;
            //SelectDirectoryButton.Click += (sender, args) => MessageBox.Show($"{viewModel.ListResults.Count}");
        }

        public void GoToCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DirectoryList.SelectedItem as ListResult)?.IsDirectory ?? false;
        }

        public void GoToCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as ViewModel)?.GoTo((DirectoryList.SelectedItem as ListResult)?.Name ?? "./");
        }
    }
}
