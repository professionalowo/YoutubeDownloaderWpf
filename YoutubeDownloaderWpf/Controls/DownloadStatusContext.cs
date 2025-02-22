using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YoutubeDownloaderWpf.Controls;

public class DownloadStatusContext : INotifyPropertyChanged
{

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private double _size = 0;
    public double Size
    {
        get => _size;
        set
        {
            _size = value;
            OnPropertyChanged();
        }
    }

    private Brush _background = Brushes.White;
    public Brush Background
    {
        get => _background;
        set
        {
            _background = value;
            OnPropertyChanged();
        }
    }

    private double _progress = 0;
    public double ProgressValue
    {
        get => _progress;
        set
        {
            _progress = value;
            OnPropertyChanged();
        }
    }

    public Progress<double> ProgressHandler { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public event EventHandler<bool> DownloadFinished;
    public DownloadStatusContext(string name, double sizeInMb)
    {
        _name = name;
        _size = Math.Round(sizeInMb, 2);
        ProgressHandler = new(p => ProgressValue = p * 100);
        DownloadFinished += OnDownloadFinished;
    }
    public void InvokeDownloadFinished(object? sender, bool status) => DownloadFinished.Invoke(sender, status);

    private void OnDownloadFinished(object? sender, bool e)
    {
        ProgressValue = 100;
        Background = e ? Brushes.LightGreen : Brushes.OrangeRed;
    }

    public Progress<long> GetProgressWrapper()
    {
        Progress<long> downloadProgress = new();
        downloadProgress.ProgressChanged += (_, e) =>
        {
            var percentage = Math.Min(e / (Size * 1000), 100);
            if (ProgressHandler is IProgress<double> p)
            {
                p.Report(percentage);
            }
        };

        return downloadProgress;
    }
}
