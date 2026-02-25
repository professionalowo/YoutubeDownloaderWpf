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
        public sealed class Flags : ICollection<string>
        {
            private readonly List<string> _inner = [];

            public IEnumerator<string> GetEnumerator() => _inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(string item) => _inner.Add(item);

            public void Clear() => _inner.Clear();

            public bool Contains(string item) => _inner.Contains(item);

            public void CopyTo(string[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

            public bool Remove(string item) => _inner.Remove(item);

            public int Count => _inner.Count;
            public bool IsReadOnly => false;
        }
    }
}