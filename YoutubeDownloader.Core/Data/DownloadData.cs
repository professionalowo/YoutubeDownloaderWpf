using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public sealed record DownloadData<TContext>(StreamData Data, TContext Context)
    where TContext : IConverter<TContext>.IConverterContext;