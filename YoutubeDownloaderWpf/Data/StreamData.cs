using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Data
{
    public readonly struct StreamData(Stream stream, string[] segments)
    {
        public string[] Segments => segments;
        public Stream Stream => stream;
    }
}
