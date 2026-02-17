using Microsoft.Extensions.Logging;

namespace YoutubeDownloader.Core.Services.Logging;

public sealed class FileLoggerProvider(string fileName) : ILoggerProvider
{
    private Stream? _stream;

    private static FileStream CreateOrOpenLogFile(string fileName)
    {
        var cwd = Directory.GetCurrentDirectory() ?? throw new IOException("Couldn't get CWD");
        var fullPath = Path.GetFullPath(Path.Combine(cwd, "logs", fileName)) ??
                       throw new IOException($"Couldn't get path to log file {fileName}");
        var dirName = Path.GetDirectoryName(fullPath) ??
                      throw new IOException("Couldn't get path to log file folder");
        Directory.CreateDirectory(dirName);
        return File.Open(
            fullPath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.ReadWrite
        );
    }

    public ILogger CreateLogger(string categoryName)
    {
        _stream = new BufferedStream(CreateOrOpenLogFile(fileName));
        return new SimpleStreamLogger(categoryName, _stream);
    }


    public void Dispose()
    {
        _stream?.Dispose();
    }
}