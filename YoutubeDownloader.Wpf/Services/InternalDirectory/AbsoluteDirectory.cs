using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Services.InternalDirectory;
public record AbsoluteDirectory(string FullPath) : IDirectory;

