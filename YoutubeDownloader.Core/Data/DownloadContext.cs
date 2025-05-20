using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using YoutubeDownloader.Core.Services.Converter;

namespace YoutubeDownloader.Core.Data;

[DebuggerDisplay("Name = {Name}, Size = {Size}")]
public class DownloadContext : INotifyPropertyChanged, IConverter<DownloadContext>.IConverterContext
{
    public string Name
    {
        get;
        protected set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    //Size is in mb
    public double Size
    {
        get;
        protected set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public virtual double ProgressValue
    {
        get;
        protected set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    private double ProgressMultiplier { get; }
    private IProgress<double> ProgressHandler { get; }

    public DownloadContext(string name, double sizeInMb, int progressMultiplier = 1)
    {
        Name = name;
        Size = Math.Round(sizeInMb, 2);
        ProgressMultiplier = progressMultiplier;
        ProgressHandler = new MultiplierHandler(this);
        DownloadFinished += OnDownloadFinished;
    }

    #region Progress

    public IProgress<long> GetProgress()
        => new DownloadProgress(this);

    private sealed class DownloadProgress : Progress<long>
    {
        private const int mb = 1000 * 1000;

        public DownloadProgress(DownloadContext ctx)
            => ProgressChanged += OnProgressChangedFactory(ctx);


        private static EventHandler<long> OnProgressChangedFactory(DownloadContext ctx)
            => (_, value) =>
            {
                var percentage = value / (ctx.Size * mb);
                var report = Math.Min(percentage, 100);
                ctx.ProgressHandler.Report(report);
            };
    }

    private sealed class MultiplierHandler(DownloadContext ctx)
        : Progress<double>(p => ctx.ProgressValue += p * ctx.ProgressMultiplier);

    #endregion

    #region DownloadFinished

    public event EventHandler<bool> DownloadFinished;

    public void InvokeDownloadFinished(object sender, bool finishedSuccessfully) =>
        DownloadFinished.Invoke(sender, finishedSuccessfully);

    protected virtual void OnDownloadFinished(object? sender, bool e)
        => ProgressValue = 1 * ProgressMultiplier;

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}