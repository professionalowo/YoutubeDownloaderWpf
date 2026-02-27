using System.Windows.Media;
using YoutubeDownloader.Core.Data;

namespace YoutubeDownloader.Wpf.Controls;

public sealed class DownloadStatusContext(string name, double sizeInMb)
    : DownloadContext(name, sizeInMb, 100)
{
    public Brush Background
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = Brushes.Transparent;

    protected override void OnDownloadFinished(object? sender, bool e)
    {
        base.OnDownloadFinished(sender, e);
        Background = e ? Brushes.LightGreen : Brushes.OrangeRed;
    }
}