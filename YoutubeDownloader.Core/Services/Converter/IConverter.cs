﻿namespace YoutubeDownloader.Core.Services.Converter;


public interface IConverter<in TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public interface IConverterContext
    {
        IProgress<long> GetProgress();
        void InvokeDownloadFinished(object sender, bool finishedSuccessfully);
    }

    public ValueTask Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default);
}
