using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Data;
using YoutubeDownloaderWpf.Services.InternalDirectory;
using YoutubeDownloaderWpf.Util.Extensions;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace YoutubeDownloaderWpf.Services.Downloader.Download;

public class VideoDownload(
    YoutubeClient client,
    string url,
    string path = "") : IDownload
{
    public string Path => path;
    public Task<string> Name => _name.Value;

    private readonly Lazy<Task<string>> _name = new(async () =>
    {
        var video = await client.Videos.GetAsync(url);
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url);
        return video.Title.ReplaceIllegalCharacters();
    });
    public async Task<DownloadData<StreamData>> GetStreamAsync(ObservableCollection<DownloadStatusContext> downloadStatuses, CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url, token);
        string name = await _name.Value;
        var streamInfo = streamManifest.GetAudioStreams().Where(s => s.Container == Container.Mp3 || s.Container == Container.Mp4).GetWithHighestBitrate();
        DownloadStatusContext statusContext = new(name.Split("/").Last(), streamInfo.Size.MegaBytes);
        _ = YoutubeDownloader.DispatchToUI(() => downloadStatuses.Add(statusContext));
        var stream = await client.Videos.Streams.GetAsync(streamInfo, token);
        return new(new(stream, [path, name]), statusContext);
    }
}
