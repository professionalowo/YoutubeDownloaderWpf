using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
    [StringSyntax(StringSyntaxAttribute.Uri)] string url,
    string path = "")
{
    public string Path => path;

    public async ValueTask<DownloadData<StreamData>> GetStreamAsync(CancellationToken token = default)
    {
        var nameTask = GetName(token);
        var streamInfo = await GetStreamInfo(token);
        var streamTask = client.Videos.Streams.GetAsync(streamInfo, token);

        var name = await nameTask;
        DownloadStatusContext statusContext = new(name, streamInfo.Size.MegaBytes);
        StreamData data = new(await streamTask, [path, name]);
        return new(data, statusContext);
    }

    private async ValueTask<IStreamInfo> GetStreamInfo(CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url, token);
        var streamInfo = streamManifest.GetAudioStreams()
            //.Where(s => s.Container == Container.Mp3 || s.Container == Container.Mp4)
            .GetWithHighestBitrate();
        return streamInfo;
    }

    public async ValueTask<string> GetName(CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(url, token);
        return video.Title.ReplaceIllegalCharacters();
    }
}
