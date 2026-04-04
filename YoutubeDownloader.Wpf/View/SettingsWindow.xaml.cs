using System.Windows;
using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Wpf.View
{
    public partial class SettingsWindow : Window
    {
        private readonly ISettingsService _settingsService;

        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.CurrentSettings;
            DownloadPathTextBox.Text = settings.DownloadPath;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = _settingsService.CurrentSettings;
            settings.DownloadPath = DownloadPathTextBox.Text;
            await _settingsService.SaveSettingsAsync(settings);
            Close();
        }
    }
}

