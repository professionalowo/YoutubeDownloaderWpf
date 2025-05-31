using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public readonly record struct DownloadData<TContext>(StreamData Data, TContext Context)
    where TContext : IConverter<TContext>.IConverterContext;