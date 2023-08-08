using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using YoutubeDownloaderWpf.Services;

namespace YoutubeDownloaderWpf.Controls
{
    public class DownloadStatusContext : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private double _sizeInMb = 0;
        private Brush _background = Brushes.White;
        private double _progress = 0;
        public DownloadStatus AsStatus => new(this);
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public double Size { get { return _sizeInMb; } set { _sizeInMb = value; OnPropertyChanged(); } }
        public Brush Background { get { return _background; } set { _background = value; OnPropertyChanged(); } }
        public double ProgressValue { get { return _progress; } set { _progress = value; OnPropertyChanged(); } }

        public Progress<double> ProgressHandler { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event EventHandler<bool> DownloadFinished;

        public CancellationTokenSource Cancellation { get; } = new();
        public DownloadStatusContext(string name, double sizeInMb)
        {
            Name = name;
            Size = Math.Round(sizeInMb, 2);
            ProgressHandler = new(p => ProgressValue = p * 100);
            DownloadFinished += OnDownloadFinished;
        }
        public void InvokeDownloadFinished(object? sender, bool status) => DownloadFinished.Invoke(sender, status);

        private void OnDownloadFinished(object? sender, bool e)
        {
            ProgressValue = 100;
            Background = Brushes.LightGreen;
        }
    }
}
