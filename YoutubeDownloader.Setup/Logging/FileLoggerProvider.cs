using Microsoft.Extensions.Logging;

namespace YoutubeDownloader.Setup.Logging;

public sealed class FileLoggerProvider(string fileName) : ILoggerProvider
{
    private Stream? _stream;

    private static FileStream CreateOrOpenLogFile(string fileName)
    {
        var dirName = Path.GetDirectoryName(fileName) ??
                      throw new IOException("Couldn't get path to log file folder");
        Directory.CreateDirectory(dirName);
        return File.Open(
            fileName,
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