using Microsoft.Extensions.Logging;

namespace YoutubeDownloader.Core.Services.Logging;

public class FileLoggerProvider(string fileName) : ILoggerProvider
{
    private static FileStream CreateOrOpenLogFile(string fileName)
    {
        string cwd = Directory.GetCurrentDirectory() ?? throw new IOException("Couldn't get CWD");
        string fullPath = Path.GetFullPath(Path.Combine(cwd, "logs", fileName)) ?? throw new IOException($"Couldn't get path to log file {fileName}");
        string dirName = Path.GetDirectoryName(fullPath) ?? throw new IOException($"Couldn't get path to log file folder");
        Directory.CreateDirectory(dirName);
        return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
    }

    public ILogger CreateLogger(string categoryName)
    => new SimpleStreamLogger(categoryName, new BufferedStream(CreateOrOpenLogFile(fileName)));

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
