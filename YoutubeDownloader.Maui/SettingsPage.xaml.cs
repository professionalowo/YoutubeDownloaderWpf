using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Maui
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ISettingsService _settingsService;

        public SettingsPage(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.CurrentSettings;
            DownloadPathEntry.Text = settings.DownloadPath;
        }

        private async void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            var settings = _settingsService.CurrentSettings;
            settings.DownloadPath = DownloadPathEntry.Text;
            await _settingsService.SaveSettingsAsync(settings);
            await Navigation.PopAsync();
        }
    }
}

