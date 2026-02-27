namespace YoutubeDownloader.Core.Services.Converter;

public interface IAudioConversionContext
{
    string Name { get; }
    IProgress<long> GetProgress();
    void InvokeDownloadFinished(object sender, bool finishedSuccessfully);
}