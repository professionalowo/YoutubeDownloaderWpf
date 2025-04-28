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
using System.IO;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using YoutubeDownloader.Wpf.Controls;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util;
using YoutubeDownloader.Core.Services.Downloader;

namespace YoutubeDownloader.Wpf.Services.Downloader;

public class YoutubeDownloader(
    ConverterFactory converterFactory,
    SystemInfo info,
    ILogger<YoutubeDownloader> logger,
    DownloadFactory<DownloadStatusContext> downloadFactory,
    IDirectory downloads)
    : YoutubeDownloaderBase<DownloadStatusContext>(converterFactory, info, logger, downloadFactory, downloads)
{
    protected override async Task DispatchToUi(Action action, CancellationToken cancellationToken = default)
        => await Dispatch(action, cancellationToken);

    protected override async Task DispatchToUi(Func<Task> action, CancellationToken token = default) =>
        await Dispatch(() => action(), token);

    protected override DownloadStatusContext ContextFactory(string name, double size)
        => new(name, size);

    static DispatcherOperation Dispatch(Action action, CancellationToken token = default) =>
        Application.Current.Dispatcher.InvokeAsync(action, DispatcherPriority.Render, token);
}