﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.Logging
{
    public class FileLoggerProvider(string fileName) : ILoggerProvider
    {
        private readonly FileStream _logFileStream = CreateOrOpenLogFile(fileName);

        private static FileStream CreateOrOpenLogFile(string fileName)
        {
            string cwd = Environment.CurrentDirectory ?? throw new IOException("Couldn't get CWD");
            string fullPath = Path.GetFullPath(Path.Combine(cwd, "logs", fileName)) ?? throw new IOException($"Couldn't get path to log file {fileName}");
            string dirName = Path.GetDirectoryName(fullPath) ?? throw new IOException($"Couldn't get path to log file folder");
            Directory.CreateDirectory(dirName);
            return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        }

        public ILogger CreateLogger(string categoryName)
        => new SimpleStreamLogger(categoryName, _logFileStream, LogLevel.Warning);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _logFileStream.Dispose();
        }
    }
}