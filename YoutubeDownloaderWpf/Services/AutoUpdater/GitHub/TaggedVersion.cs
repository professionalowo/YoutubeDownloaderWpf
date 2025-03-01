using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater.GitHub;

public record TaggedVersion(uint Major, uint Minor, uint Patch) : IComparable<TaggedVersion>
{
    public int CompareTo(TaggedVersion? other)
    => other switch
    {
        null => 1,
        _ when Major != other.Major => Major.CompareTo(other.Major),
        _ when Minor != other.Minor => Minor.CompareTo(other.Minor),
        _ => Patch.CompareTo(other.Patch),
    };

    public static TaggedVersion FromTag(string tag)
    {
        if (tag.Split('.').Select(uint.Parse).Take(3).ToArray() is [uint major, uint minor, uint patch])
        {
            return new(major, minor, patch);
        }
        throw new ArgumentException("Tag did not conform to the pattern {major}.{minor}.{patch}");
    }

    public override string ToString() => $"v{Major}.{Minor}.{Patch}";
}
