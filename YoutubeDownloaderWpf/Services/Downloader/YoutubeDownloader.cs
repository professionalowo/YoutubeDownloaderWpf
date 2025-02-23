using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using YoutubeDownloaderWpf.Controls;
using System.IO;
using System.Windows.Threading;
using YoutubeDownloaderWpf.Services.Converter;
using System.Threading;
using Microsoft.Extensions.Logging;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Services.Downloader.Download;
using YoutubeDownloaderWpf.Util;



namespace YoutubeDownloaderWpf.Services.Downloader;

public class YoutubeDownloader(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloader> logger,
    DownloadFactory downloadFactory,
    IDirectory downlaods) : IDownloader, INotifyPropertyChanged
{

    private string _url = string.Empty;
    public string Url
    {
        get => _url;
        set { _url = value; OnPropertyChanged(); }
    }

    private bool _forceMp3 = true;
    public bool ForceMp3
    {

        get => _forceMp3;
        set
        {
            _forceMp3 = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<DownloadStatusContext> DownloadStatuses { get; } = [];
    private CancellationTokenSource _cancellationSource = new();

    public string DownloadDirectoryPath => downlaods.FullPath;


    public async Task Download()
    {
        await DispatchToUI(DownloadStatuses.Clear);
        await DownloadAction(Url);
    }

    public async Task Cancel()
    {
        _cancellationSource.Cancel();
        _cancellationSource = new();
        await DispatchToUI(DownloadStatuses.Clear);
    }

    private async Task DownloadAction(string url)
    {
        try
        {
            CancellationToken token = _cancellationSource.Token;
            IConverter converter = converterFactory.GetGonverter(ForceMp3);
            List<Task> tasks = new(20);
            SemaphoreSlim semaphoreSlim = new(info.Cores);
            await foreach (var download in downloadFactory.Get(url))
            {
                var streamTask = Task.Run(async () => await download.GetStreamAsync())
                    .ContinueWith(async (resolveTask) =>
                {
                    var (data, context) = await resolveTask;
                    await DispatchToUI(() => DownloadStatuses.Add(context), token);
                    string fileName = downlaods.ChildFileName(data.Segments);
                    await semaphoreSlim.WaitAsync(token);
                    await using Stream mediaStream = data.Stream;
                    await converter.Convert(mediaStream, fileName, context, token);
                    semaphoreSlim.Release();
                });
                tasks.Add(streamTask);
            }
            await Task.WhenAll(tasks);
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public static DispatcherOperation DispatchToUI(Action action, CancellationToken token = default) => Application.Current.Dispatcher.InvokeAsync(action, DispatcherPriority.Render, token);
    public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
}
