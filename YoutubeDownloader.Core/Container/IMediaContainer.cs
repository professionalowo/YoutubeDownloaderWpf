using System.Collections;

namespace YoutubeDownloader.Core.Container;

public interface IMediaContainer
{
    FileExtension Extension { get; }
    Codec FfmpegCodec { get; }
    Codec.Flags FfmpegCodecFlags { get; }
    
    readonly record struct FileExtension(string Extension);

    readonly record struct Codec(string FfmpegCodec)
    {
        public readonly record struct Flags(ICollection<string> FfmpegCodecFlags) : ICollection<string>
        {
            public IEnumerator<string> GetEnumerator()
            {
                return FfmpegCodecFlags.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(string item)
            {
                FfmpegCodecFlags.Add(item);
            }

            public void Clear()
            {
                FfmpegCodecFlags.Clear();
            }

            public bool Contains(string item)
            {
                return FfmpegCodecFlags.Contains(item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                FfmpegCodecFlags.CopyTo(array, arrayIndex);
            }

            public bool Remove(string item)
            {
                return FfmpegCodecFlags.Remove(item);
            }

            public int Count => FfmpegCodecFlags.Count;
            public bool IsReadOnly => FfmpegCodecFlags.IsReadOnly;
        }
    }
}