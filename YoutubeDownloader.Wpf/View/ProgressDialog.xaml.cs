using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace YoutubeDownloader.Wpf.View;

public partial class ProgressDialog : Window, INotifyPropertyChanged
{
    public double ProgressValue
    {
        get;
        set
        {
            OnPropertyChanged();
            field = value;
        }
    }

    public string Message
    {
        get;
        set
        {
            OnPropertyChanged();
            field = value;
        }
    }

    public Progress<double> Progress { get; }

    public ProgressDialog()
    {
        DataContext = this;
        Progress = new Progress<double>(v =>
        {
            ProgressValue = v;
            Message = FormatMessage(v);
        });
        Message = FormatMessage(0);
        InitializeComponent();
    }

    private static string FormatMessage(double progress)
    {
        var percentage = progress * 100;
        return $"Downloading... ({percentage:##00.0}%)";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}