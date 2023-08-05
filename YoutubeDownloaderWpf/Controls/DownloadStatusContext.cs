using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YoutubeDownloaderWpf.Services;

namespace YoutubeDownloaderWpf.Controls
{
    public class DownloadStatusContext : INotifyPropertyChanged
    {
        private string _name;
        private double _sizeInMb;
        private bool _isCompleted;
        private Brush _background = Brushes.White;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public double Size { get { return _sizeInMb; } set { _sizeInMb = value; OnPropertyChanged(); } }
        public bool IsCompleted { get { return _isCompleted; } set { _isCompleted = value; OnPropertyChanged(); } }

        public Brush Background {get { return _background; } set {_background = value; OnPropertyChanged(); } } 

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event EventHandler<bool> DownloadFinished;
        public DownloadStatusContext(string name, double sizeInMb)
        {

            Name = name;
            Size = Math.Round(sizeInMb,2);
            IsCompleted = false;
            DownloadFinished += OnDownloadFinished;
        }
            
        public void OnDownloadFinished(object? sender, bool e)
        {
            IsCompleted = e;
            Background = Brushes.LightGreen;
        }
    }
}
