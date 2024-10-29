using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.Logging
{
    public class SimpleStreamLogger(string loggerName, Stream outStream, LogLevel minLogLevel) : ILogger, IDisposable
    {
        private readonly string _loggerName = loggerName;
        private readonly LogLevel _minLogLevel = minLogLevel;
        private readonly StreamWriter _writer = new(outStream);

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
        => logLevel >= _minLogLevel && logLevel != LogLevel.None;

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            await _writer.WriteLineAsync($"[{_loggerName}]({logLevel}): {formatter(state, exception)}");
        }
    }
}
