using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Wpf.View
{
    public partial class SettingsWindow : Window
    {
        private readonly ISettingsService _settingsService;
        public AppConfiguration? Settings { get; set; }

        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            Loaded += async (s, e) => await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Settings = await _settingsService.LoadSettingsAsync();
            DataContext = this;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings is not null)
            {
                await _settingsService.SaveSettingsAsync(Settings);
            }
            Close();
        }
    }
}
