using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Util;

public class SystemInfo
{
    /// <summary>
    /// Use 80% of cores
    /// </summary>
    public int Cores => (int)Math.Ceiling(Environment.ProcessorCount * 0.8);
}
