﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloader.Wpf.Services.Logging;

public class SimpleStreamLogger(string loggerName, Stream outStream) : ILogger, IDisposable
{
    private readonly Lock _writerLock = new();
    private readonly StreamWriter _writer = new(outStream) { AutoFlush = true };

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _writer.Dispose();
    }

    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string formatted = $"[{loggerName}]({logLevel}): {formatter(state, exception)}";
        lock (_writerLock)
        {
            _writer.WriteLine(formatted);
        }
    }
}