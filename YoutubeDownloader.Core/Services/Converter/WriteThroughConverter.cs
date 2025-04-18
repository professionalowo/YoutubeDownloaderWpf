﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Util.Extensions;

namespace YoutubeDownloader.Core.Services.Converter;

public class WriteThroughConverter(string extension) : IConverter
{
    public async ValueTask<string> Convert(Stream data, string outPath, IConverter.IConverterContext context, CancellationToken token = default)
    {
        FileInfo fileInfo = new(Path.ChangeExtension(outPath, extension));
        try
        {
            await using FileStream file = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            await data.CopyToAsyncTracked(file, context.GetProgress(), token);
            context.InvokeDownloadFinished(this, true);
            return fileInfo.FullName;
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
