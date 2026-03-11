using System.Runtime.CompilerServices;
using SoundCloudExplode;
using SoundCloudExplode.Common;
using YoutubeDownloader.Core.Data.Download;
using YoutubeDownloader.Core.Extensions;
using YoutubeDownloader.Core.Services.InternalDirectory;

namespace YoutubeDownloader.Core.Services.Downloader.Platform.SoundCloud;

public class SoundCloudPlatformService(HttpClient http, SoundCloudClient client, IDirectory downloads)
    : IPlatformService
{
    public async IAsyncEnumerable<IVideoDownload> Get(string url,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await client.InitializeAsync(token)
            .ConfigureAwait(false);
        var kind = await client.GetUrlKindAsync(url, token)
            .ConfigureAwait(false);

        switch (kind)
        {
            case Kind.Track:
                yield return new SingleVideoDownload(url);
                break;
            case Kind.Playlist or Kind.Album:
            {
                var playlist = await client.Playlists.GetAsync(url, false, token);
                var title = playlist.Title ?? throw new InvalidOperationException();
                var name = title.ReplaceIllegalFileNameCharacters();
                await downloads.CreateSubDirectoryAsync(name);
                var enumerable = client.Playlists.GetTracksAsync(url, token)
                    .ConfigureAwait(false);
                await foreach (var track in enumerable)
                {
                    var trackUrl = track.Uri?.ToString();
                    if (trackUrl is null) continue;
                    if (await client.Tracks.IsUrlValidAsync(trackUrl, token))
                        yield return new PlaylistVideoDownload(name, trackUrl);
                }

                break;
            }
        }
    }


    public async Task<NamedVideoDownload> GetName(IVideoDownload download, CancellationToken token = default)
    {
        var track = await client.Tracks.GetAsync(download.Url, token) ?? throw new InvalidOperationException();
        var title = track.Title ?? throw new InvalidOperationException();
        var name = title.ReplaceIllegalFileNameCharacters();
        return new NamedVideoDownload(download, name);
    }

    public string GetFileName(NamedVideoDownload named)
    {
        var (download, title) = named;
        var formatted = download.FormatName(title);
        return downloads.ChildFileName(formatted);
    }

    private sealed record SoundCloudPlatformStreamInfo(double SizeInMb, string Url) : IPlatformStreamInfo
    {
        public object Underlying => Url;
    }

    public async Task<StreamVideoDownload> GetStreamInfo(IVideoDownload download, CancellationToken token = default)
    {
        var downloadUrl = await client.Tracks.GetDownloadUrlAsync(download.Url, token)
            .ConfigureAwait(false);

        if (downloadUrl is null)
            throw new InvalidOperationException("This track is not downloadable");


        using var response = await http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;

        var sizeMb = (contentLength ?? 0) / 1024.0 / 1024.0;
        return new StreamVideoDownload(download, new SoundCloudPlatformStreamInfo(sizeMb, downloadUrl));
    }

    public async Task<Stream> GetStream(StreamVideoDownload download, CancellationToken token = default)
    {
        var downloadUrl = await client.Tracks.GetDownloadUrlAsync(download.Download.Url, token)
            .ConfigureAwait(false);

        if (downloadUrl is null)
            throw new InvalidOperationException("This track is not downloadable");

        var response = await http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, token)
            .ConfigureAwait(false);
        return await ResponseBackedStream.CreateAsync(response)
            .ConfigureAwait(false);
    }
}