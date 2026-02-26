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
        public sealed class Flags : IList<Flag>
        {
            private readonly List<Flag> _inner = [];

            public IEnumerator<Flag> GetEnumerator() => _inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(Flag item) => _inner.Add(item);

            public void Clear() => _inner.Clear();

            public bool Contains(Flag item) => _inner.Contains(item);

            public void CopyTo(Flag[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

            public bool Remove(Flag item) => _inner.Remove(item);

            public int Count => _inner.Count;
            public bool IsReadOnly => false;

            public int IndexOf(Flag item) => _inner.IndexOf(item);

            public void Insert(int index, Flag item) => _inner.Insert(index, item);

            public void RemoveAt(int index) => _inner.RemoveAt(index);

            public Flag this[int index]
            {
                get => _inner[index];
                set => _inner[index] = value;
            }

            public IEnumerable<string> Format() => _inner.SelectMany(flag => flag.Format());
        }

        public readonly record struct Flag(string Name, string Value)
        {
            public static Flag Create<T>(string name, T value) where T : notnull =>
                new(name, value.ToString() ?? throw new InvalidOperationException());

            internal IEnumerable<string> Format() => [$"-{Name}", Value];
        }
    }
}