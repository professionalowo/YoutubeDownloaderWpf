using System.Windows;
using System.Windows.Controls;

namespace YoutubeDownloader.Wpf.View
{
    public partial class ShellWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private readonly SettingsWindow _settingsWindow;

        public ShellWindow(MainWindow mainWindow, SettingsWindow settingsWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _settingsWindow = settingsWindow;
            MainFrame.Navigate(_mainWindow);
        }

        private void NavigateToDownload_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_mainWindow);
        }

        private void NavigateToSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_settingsWindow);
        }
    }
}

