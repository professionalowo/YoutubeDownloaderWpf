using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Core.Services.InternalDirectory
{
    public class ChildDirectory(IDirectory parent,string name) : IDirectory
    {
        public string FullPath => Path.Combine(parent.FullPath, name);
    }
}
