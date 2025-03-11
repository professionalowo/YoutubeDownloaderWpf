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
using System.Diagnostics.CodeAnalysis;



namespace YoutubeDownloaderWpf.Services.Downloader;

public class YoutubeDownloader(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloader> logger,
    DownloadFactory downloadFactory,
    IDirectory downloads) : IDownloader, INotifyPropertyChanged
{

    private string _url = string.Empty;

    [StringSyntax(StringSyntaxAttribute.Uri)]
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

    public async Task Download()
    {
        await DispatchToUI(DownloadStatuses.Clear);
        await DownloadAction(Url, _cancellationSource.Token).ConfigureAwait(false);
    }

    public async Task Cancel()
    {
        await _cancellationSource.CancelAsync();
        await DispatchToUI(DownloadStatuses.Clear);
        _cancellationSource = new();
    }

    private async Task DownloadAction([StringSyntax(StringSyntaxAttribute.Uri)] string url, CancellationToken token = default)
    {
        try
        {
            IConverter converter = converterFactory.GetConverter(ForceMp3);
            List<Task> tasks = new(20);
            SemaphoreSlim semaphoreSlim = new(info.Cores);
            await foreach (var download in downloadFactory.Get(url))
            {
                var streamTask = Task.Run(async () => await download.GetStreamAsync(token).ConfigureAwait(false))
                    .ContinueWith(async (resolveTask) =>
                {
                    var (data, context) = await resolveTask;
                    string fileName = downloads.ChildFileName(data.Segments);
                    var uiTask = DispatchToUI(() => DownloadStatuses.Add(context), token);
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
                    await uiTask;
                    await using Stream mediaStream = data.Stream;
                    await converter.Convert(mediaStream, fileName, context, token).ConfigureAwait(false);
                    semaphoreSlim.Release();
                }, token);
                tasks.Add(streamTask);
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            logger.LogError("{Error}", e);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public static DispatcherOperation DispatchToUI(Action action, CancellationToken token = default) => Application.Current.Dispatcher.InvokeAsync(action, DispatcherPriority.Render, token);
    public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
}
