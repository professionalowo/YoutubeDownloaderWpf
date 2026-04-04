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

        private void ToggleNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavPanel.Visibility = NavPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ListBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "Download")
                {
                    MainFrame.Navigate(_mainWindow);
                }
                else if (selectedItem.Content.ToString() == "Settings")
                {
                    MainFrame.Navigate(_settingsWindow);
                }

                NavPanel.Visibility = Visibility.Collapsed;
                ((ListBox)sender).SelectedItem = null; // Deselect item
            }
        }
    }
}
