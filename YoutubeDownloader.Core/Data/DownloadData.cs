﻿using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

public readonly record struct DownloadData<TData, TContext>(TData Data, TContext Context)
    where TContext : IConverter<TContext>.IConverterContext;