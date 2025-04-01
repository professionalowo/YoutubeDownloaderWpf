using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Wpf.Services.InternalDirectory;
using YoutubeDownloader.Wpf.Controls;
using YoutubeDownloader.Wpf.Data;
using YoutubeDownloader.Wpf.Util.Extensions;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace YoutubeDownloader.Wpf.Services.Downloader.Download;

public class PlaylistDownload(
    YoutubeClient client,
    [StringSyntax(StringSyntaxAttribute.Uri)] string url,
    IDirectory downloads) : IAsyncEnumerable<VideoDownload>
{
    public async IAsyncEnumerator<VideoDownload> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var playlist = await client.Playlists.GetAsync(url, cancellationToken);
        var dir = await downloads.CreateSubDirectoryAsync(playlist.Title.ReplaceIllegalCharacters());
        await foreach (var video in client.Playlists.GetVideosAsync(url, cancellationToken))
        {
            yield return new VideoDownload(client, video.Url, dir.Name);
        }
    }
}
