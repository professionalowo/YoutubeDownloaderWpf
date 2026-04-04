using System.Threading.Tasks;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services;

namespace YoutubeDownloader.Maui
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ISettingsService _settingsService;
        public AppConfiguration? Settings { get; set; }

        public SettingsPage(ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Settings = await _settingsService.LoadSettingsAsync();
            BindingContext = this;
        }

        private async void SaveButton_Clicked(object sender, System.EventArgs e)
        {
            if (Settings is not null)
            {
                await _settingsService.SaveSettingsAsync(Settings);
            }
            await Navigation.PopAsync();
        }
    }
}
