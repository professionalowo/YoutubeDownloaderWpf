using YoutubeDownloader.Core.Container;

namespace YoutubeDownloader.Core.Services.Converter;

public interface IConverter
{
    public interface IConverterContext
    {
        string Name { get; }
        IProgress<long> GetProgress();
        void InvokeDownloadFinished(object sender, bool finishedSuccessfully);
    }

    public Task Convert(Stream audioStream, string outPath, IConverterContext context,
        CancellationToken token = default);
}