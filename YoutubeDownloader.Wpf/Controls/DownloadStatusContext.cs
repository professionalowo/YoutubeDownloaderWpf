using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Wpf.Controls;

public sealed class DownloadStatusContext(string name, double sizeInMb)
    : DownloadContext(name, sizeInMb)
{
    public Brush Background
    {
        get;
        set
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