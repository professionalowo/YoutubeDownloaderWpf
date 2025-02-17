﻿using System;
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
using System.CodeDom;
using Microsoft.Extensions.Logging;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Services.Downloader.Download;
using YoutubeDownloaderWpf.Data;
using YoutubeDownloaderWpf.Services.AutoUpdater;
using System.IO.Pipelines;
using YoutubeDownloaderWpf.Util.Extensions;
using YoutubeDownloaderWpf.Util;
using System.Xml.Linq;


namespace YoutubeDownloaderWpf.Services.Downloader
{
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
        public CancellationTokenSource CancellationSource { private set; get; } = new();

        public string DownloadDirectoryPath => downlaods.FullPath;


        public async Task Download()
        {
            await DispatchToUI(DownloadStatuses.Clear);
            await DownloadAction(Url);
        }



        private async Task DownloadAction(string url)
        {
            try
            {
                CancellationSource = new();
                CancellationToken token = CancellationSource.Token;
                IConverter converter = converterFactory.GetGonverter(ForceMp3);
                List<Task> tasks = new(20);
                SemaphoreSlim semaphoreSlim = new(info.Cores);
                await foreach (IDownload download in downloadFactory.Get(url))
                {
                    var streamTask = Task.Run(async () => await download.GetStreamAsync(DownloadStatuses));
                    var continuationTask = streamTask.ContinueWith(async (resolveTask) =>
                    {
                        var (data, context) = await resolveTask;
                        string fileName = downlaods.SaveFileName(data.Segments);
                        await semaphoreSlim.WaitAsync(token);
                        await using var mediaStream = data.Stream;
                        await converter.Convert(mediaStream, fileName, context, token);
                        semaphoreSlim.Release();
                    });
                    tasks.Add(continuationTask);
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

        private async Task Download(string url, CancellationToken token = default)
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static DispatcherOperation DispatchToUI(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
        public static void DispatchToUISync(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
    }
}
