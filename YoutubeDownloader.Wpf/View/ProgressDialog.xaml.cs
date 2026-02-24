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
            field = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public Progress<double> Progress { get; }

    public ProgressDialog()
    {
        DataContext = this;
        Progress = new Progress<double>(v =>
        {
            ProgressValue = v;
            Message = v <= 0.8 ? FormatMessage("Downloading", v) : FormatMessage("Extracting", v);
        });
        Message = FormatMessage("Initializing", 0);
        InitializeComponent();
    }

    private static string FormatMessage(ReadOnlySpan<char> message, double progress)
    {
        var percentage = progress * 100;
        return $"{message}... ({percentage:##00.0}%)";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}