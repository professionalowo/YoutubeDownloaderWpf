using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public record DownloadData<TContext>(StreamData Data, TContext Context)
    where TContext : IConverter<TContext>.IConverterContext;