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
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Wpf.Data;


namespace YoutubeDownloader.Core.Services.Downloader;

public abstract class YoutubeDownloader<TContext>(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloader<TContext>> logger,
    DownloadFactory<TContext> downloadFactory,
    IDirectory downloads) : IDownloader<TContext>, INotifyPropertyChanged where TContext : IConverter.IConverterContext
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
    public ObservableCollection<TContext> DownloadStatuses { get; } = [];
    private readonly Lock _cancellationSourceLock = new();
    private CancellationTokenSource _cancellationSource = new();
    protected CancellationTokenSource CancellationSource
    {
        get {
            lock (_cancellationSourceLock)
            {
                return _cancellationSource;
            }
        }
        set {
            lock (_cancellationSourceLock)
            {
                _cancellationSource = value;
            }
        }
    }
    protected abstract Task DispatchToUi(Action action, CancellationToken token = default);
    
    public async Task Download()
    {
        await DispatchToUi(DownloadStatuses.Clear);
        await DownloadAction(Url, CancellationSource.Token).ConfigureAwait(false);
    }

    public async Task Cancel()
    {
        await CancellationSource.CancelAsync().ConfigureAwait(false);
        await DispatchToUi(DownloadStatuses.Clear);
        CancellationSource = new();
    }

    protected abstract TContext ContextFactory(string name, double size);
    
    protected async Task DownloadAction([StringSyntax(StringSyntaxAttribute.Uri)] string url, CancellationToken token = default)
    {
        try
        {
            IConverter converter = converterFactory.GetConverter(ForceMp3);
            List<Task> tasks = new(20);
            SemaphoreSlim semaphoreSlim = new(info.Cores);
            await foreach (var download in downloadFactory.Get(url))
            {
                Func<Task<DownloadData<StreamData,TContext>>> downloadAction = async () => await download.GetStreamAsync(ContextFactory,token).ConfigureAwait(false);
                var streamTask = Task.Run(downloadAction,token)
                    .ContinueWith((async (resolveTask) =>
                {
                    var (data, context) = await resolveTask;
                    string fileName = downloads.ChildFileName(data.Segments);
                    var uiTask = DispatchToUi(() => DownloadStatuses.Add(context), token);
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
                    await uiTask;
                    await using Stream mediaStream = data.Stream;
                    await converter.Convert(mediaStream, fileName, context, token).ConfigureAwait(false);
                    semaphoreSlim.Release();
                }), token);
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
}
