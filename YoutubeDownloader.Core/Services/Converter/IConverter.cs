using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services.Converter;


public interface IConverter<in TContext> where TContext : IConverter<TContext>.IConverterContext
{
    public interface IConverterContext
    {
        IProgress<long> GetProgress();
        void InvokeDownloadFinished(object sender, bool finishedSuccessfully);
    }

    public ValueTask<string> Convert(Stream audioStream, string outPath, TContext context,
        CancellationToken token = default);
}
