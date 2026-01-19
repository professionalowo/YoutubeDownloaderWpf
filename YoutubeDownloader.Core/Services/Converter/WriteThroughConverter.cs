using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public sealed class WriteThroughConverter<TContext>(string extension)
    : IConverter<TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public async ValueTask Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default)
    {
        var fileInfo = new FileInfo(Path.ChangeExtension(outPath, extension));
        try
        {
            await using var file = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write,
                    FileShare.ReadWrite | FileShare.Delete)
                .WithProgress(context.GetProgress());
            await audioStream.CopyToAsync(file, token)
                .ConfigureAwait(false);
            context.InvokeDownloadFinished(this, true);
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            context.InvokeDownloadFinished(this, false);
            fileInfo.Delete();
            throw;
        }
        catch (Exception)
        {
            fileInfo.Delete();
            throw;
        }
    }
}