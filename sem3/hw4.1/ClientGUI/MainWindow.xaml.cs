using System;
using System.Windows;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void OnConnection(object sender, EventArgs args)
        {
            Content = new MainPage();
            (DataContext as ViewModel)?.Connect();
        }
    }
}