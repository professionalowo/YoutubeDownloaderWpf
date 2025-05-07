using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

[DebuggerDisplay("Name = {Name},Size = {Size}")]
public class DownloadContext : INotifyPropertyChanged, IConverter<DownloadContext>.IConverterContext
{
    public string Name
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double Size
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double ProgressValue
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    private IProgress<double> ProgressHandler { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event EventHandler<bool> DownloadFinished;

    public void InvokeDownloadFinished(object? sender, bool finishedSuccessfully) =>
        DownloadFinished.Invoke(sender, finishedSuccessfully);

    protected virtual void OnDownloadFinished(object? sender, bool e)
    {
        ProgressValue = 100;
    }

    public DownloadContext(string name, double sizeInMb)
    {
        Name = name;
        Size = Math.Round(sizeInMb, 2);
        ProgressHandler = new Progress<double>(p => ProgressValue = p * 100);
        DownloadFinished += OnDownloadFinished;
    }


    public IProgress<long> GetProgress()
    {
        Progress<long> downloadProgress = new();
        downloadProgress.ProgressChanged += (_, e) =>
        {
            var percentage = Math.Min(e / (Size * 1000), 100);
            ProgressHandler.Report(percentage);
        };

        return downloadProgress;
    }
}