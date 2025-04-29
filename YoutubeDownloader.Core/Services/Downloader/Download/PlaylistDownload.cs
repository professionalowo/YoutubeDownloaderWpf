using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using YoutubeDownloader.Core.Services.Converter;
using YoutubeDownloader.Core.Services.Downloader.Download;
using YoutubeDownloader.Core.Services.InternalDirectory;
using YoutubeDownloader.Core.Util.Extensions;
using YoutubeExplode;

namespace YoutubeDownloader.Core.Services.Downloader.Download;

public class PlaylistDownload<TContext>(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)]
    string url,
    IDirectory downloads)
    : IAsyncEnumerable<VideoDownload<TContext>> where TContext : IConverter<TContext>.IConverterContext
{
    public async IAsyncEnumerator<VideoDownload<TContext>> GetAsyncEnumerator(
        CancellationToken token = default)
    {
        var playlist = await client.Playlists.GetAsync(url, token)
            .ConfigureAwait(false);
        var dir = await downloads.CreateSubDirectoryAsync(playlist.Title.ReplaceIllegalCharacters())
            .ConfigureAwait(false);
        var enumerable = client.Playlists.GetVideosAsync(url, token)
            .ConfigureAwait(false);
        await foreach (var video in enumerable)
        {
            yield return new VideoDownload<TContext>(client, video.Url, dir.Name);
        }
    }
}