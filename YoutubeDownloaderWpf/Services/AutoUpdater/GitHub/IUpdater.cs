using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater.GitHub;

public interface IUpdater
{
    public ValueTask<bool> IsNewVersionAvailable(CancellationToken token = default);
    public ValueTask UpdateVersion(CancellationToken token = default);
}
