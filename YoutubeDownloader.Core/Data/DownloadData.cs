using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public sealed record DownloadData(StreamData Data, IAudioConversionContext Context);