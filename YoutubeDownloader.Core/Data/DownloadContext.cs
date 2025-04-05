using System.ComponentModel;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public class DownloadContext : INotifyPropertyChanged, IConverter.IConverterContext
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

    public Progress<double> ProgressHandler { get; private init; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public event EventHandler<bool> DownloadFinished;
    public DownloadContext(string name, double sizeInMb)
    {
        Name = name;
        Size = Math.Round(sizeInMb, 2);
        ProgressHandler = new(p => ProgressValue = p * 100);
        DownloadFinished += OnDownloadFinished;
    }
    public void InvokeDownloadFinished(object? sender, bool status) => DownloadFinished.Invoke(sender, status);

    protected virtual void OnDownloadFinished(object? sender, bool e)
    {
        ProgressValue = 100;
    }

    public IProgress<long> GetProgress()
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