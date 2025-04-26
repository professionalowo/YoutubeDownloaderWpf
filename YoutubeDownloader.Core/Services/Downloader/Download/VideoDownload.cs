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
using YoutubeDownloader.Core.Data;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util.Extensions;
using YoutubeDownloader.Core.Data;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public class VideoDownload<TContext>(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string url,
    string path = "") where TContext : IConverter.IConverterContext
{
    public string Path => path;

    public async ValueTask<DownloadData<StreamData, TContext>> GetStreamAsync(
        Func<string, double, TContext> contextFactory, CancellationToken token = default)
    {
        var nameTask = GetName(token)
            .ConfigureAwait(false);
        var streamInfo = await GetStreamInfo(token)
            .ConfigureAwait(false);
        var streamTask = client.Videos.Streams.GetAsync(streamInfo, token)
            .ConfigureAwait(false);

        var name = await nameTask;
        var statusContext = contextFactory(name, streamInfo.Size.MegaBytes);
        var stream = await streamTask;
        var data = new StreamData(stream, [path, name]);
        return new DownloadData<StreamData, TContext>(data, statusContext);
    }

    private async ValueTask<IStreamInfo> GetStreamInfo(CancellationToken token = default)
    {
        var streamManifest = await client.Videos.Streams.GetManifestAsync(url, token).ConfigureAwait(false);
        var streamInfo = streamManifest.GetAudioStreams()
            //.Where(s => s.Container == Container.Mp3 || s.Container == Container.Mp4)
            .GetWithHighestBitrate();
        return streamInfo;
    }

    private async ValueTask<string> GetName(CancellationToken token = default)
    {
        var video = await client.Videos.GetAsync(url, token).ConfigureAwait(false);
        return video.Title.ReplaceIllegalCharacters();
    }
}