using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Data;
public readonly record struct StreamData(Stream Stream, string[] Segments);

