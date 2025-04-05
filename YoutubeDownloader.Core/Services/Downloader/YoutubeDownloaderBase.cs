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

public abstract class YoutubeDownloaderBase<TContext>(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloaderBase<TContext>> logger,
    DownloadFactory<TContext> downloadFactory,
    IDirectory downloads) : IDownloader<TContext>, INotifyPropertyChanged where TContext : IConverter.IConverterContext
{
    private readonly Lock _cancellationSourceLock = new();
    private readonly Lock _statusesLock = new();
    
    
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public string Url
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "";
    public bool ForceMp3
    {

        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = true;
    public ObservableCollection<TContext> DownloadStatuses
    {
        get
        {
            lock (_statusesLock)
            {
                return field;
            }
        }
        set
        {
            lock (_statusesLock)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = [];
    private CancellationTokenSource CancellationSource
    {
        get
        {
            lock (_cancellationSourceLock)
            {
                return field;
            }
        }
        set
        {
            lock (_cancellationSourceLock)
            {
                field = value;
            }
        }
    } = new();
    
    
    protected abstract Task DispatchToUi(Action action, CancellationToken token = default);
    
    public async Task Download()
    {
        await DispatchToUi(ClearStatuses).ConfigureAwait(false);
        await DownloadAction(Url, CancellationSource.Token).ConfigureAwait(false);
    }


    private void ClearStatuses()
    {
        DownloadStatuses.Clear();
        OnPropertyChanged(nameof(DownloadStatuses));
    }
    
    public async Task Cancel()
    {
        await CancellationSource.CancelAsync().ConfigureAwait(false);
        await DispatchToUi(ClearStatuses).ConfigureAwait(false);
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
                    await DispatchToUi(() =>
                    {
                        lock (_statusesLock)
                        {
                            DownloadStatuses.Add(context);
                        }
                    }, token).ConfigureAwait(false);
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
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
