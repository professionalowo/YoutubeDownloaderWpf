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
        public readonly struct Flags(ICollection<string> inner) : ICollection<string>
        {
            public IEnumerator<string> GetEnumerator() => inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(string item) => inner.Add(item);

            public void Clear() => inner.Clear();

            public bool Contains(string item) => inner.Contains(item);

            public void CopyTo(string[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);

            public bool Remove(string item) => inner.Remove(item);

            public int Count => inner.Count;
            public bool IsReadOnly => inner.IsReadOnly;
        }
    }
}