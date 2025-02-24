using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;
using YoutubeDownloaderWpf.Util.Extensions;

namespace YoutubeDownloaderWpf.Services.Converter;

public class WriteThroughConverter(string extension) : IConverter
{
    public async ValueTask<string?> Convert(Stream data, string outPath, DownloadStatusContext context, CancellationToken token = default)
    {
        FileInfo fileInfo = new(Path.ChangeExtension(outPath, extension));
        try
        {
            await using FileStream file = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            await data.CopyToAsyncTracked(file, context.GetProgressWrapper(), token);
            context.InvokeDownloadFinished(this, true);
            return fileInfo.FullName;
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            context.InvokeDownloadFinished(this, false);
            fileInfo.Delete();
        }
        catch (Exception)
        {
            fileInfo.Delete();
            throw;
        }
        return null;
    }
}
