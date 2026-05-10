using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services;
using YoutubeDownloader.Setup.Settings;

namespace YoutubeDownloader.Wpf.View
{
    public partial class SettingsWindow : Page, INotifyPropertyChanged
    {
        private readonly ISettingsService _settingsService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AppConfiguration? Settings
        {
            get;
            set
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Settings)));
            }
        }


        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            DataContext = this;
            Loaded += async (s, e) => await LoadSettingsAsync();
            Unloaded += async (s, e) => await SaveSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Settings = await _settingsService.LoadSettingsAsync();
        }

        private async Task SaveSettingsAsync()
        {
            if (Settings is not null)
            {
                await _settingsService.SaveSettingsAsync(Settings);
            }

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Settings is not null && !string.IsNullOrWhiteSpace(Settings.DownloadPath))
            {
                dialog.SelectedPath = Settings.DownloadPath;
            }

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            Settings?.DownloadPath = dialog.SelectedPath;
        }
    }
}