﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.InternalDirectory;

public class CwdDirectory(string name) : IDirectory
{
    public string FullPath { get; init; } = Path.Combine(Directory.GetCurrentDirectory(), name);
}
