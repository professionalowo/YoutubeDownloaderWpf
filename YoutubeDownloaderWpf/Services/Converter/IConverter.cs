using System;
using System.Collections.Generic;
<<<<<<< HEAD
<<<<<<< HEAD
using System.IO;
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloaderWpf.Controls;

namespace YoutubeDownloaderWpf.Services.Converter
{
    internal interface IConverter
    {
<<<<<<< HEAD
<<<<<<< HEAD

=======
        Task RunConversion(string filePath,DownloadStatusContext context);
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
=======
        Task RunConversion(string filePath,DownloadStatusContext context);
>>>>>>> 8ce1c53 (added option to force files to mp3, converting them from the source)
    }
}
