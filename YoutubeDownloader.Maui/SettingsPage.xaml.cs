using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Maui
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ISettingsService _settingsService;

        public AppConfiguration? Settings
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        }

        public SettingsPage(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Settings = await _settingsService.LoadSettingsAsync();
        }

        private async void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            if (Settings is not null)
            {
                await _settingsService.SaveSettingsAsync(Settings);
            }

            await Navigation.PopAsync();
        }

        private async void BrowseButton_Clicked(object sender, System.EventArgs e)
        {
            var cancellationToken = CancellationToken.None;
            var result = await FolderPicker.Default.PickAsync(cancellationToken);
            if (!result.IsSuccessful || Settings is null) return;
            Settings.DownloadPath = result.Folder.Path;
            OnPropertyChanged(nameof(Settings));
        }
    }
}