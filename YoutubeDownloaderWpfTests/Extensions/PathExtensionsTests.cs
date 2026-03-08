using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloaderWpfTests.Extensions;

[TestClass]
public class PathExtensionsTests
{
    [TestMethod]
    public void ReplaceIllegalCharactersTest_NoIllegalChars_IsIdentical()
    {
        const string expected = "hello world";
        var actual = expected.ReplaceIllegalFileNameCharacters();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ReplaceIllegalCharactersTest_IllegalChars_AreReplaced()
    {
        const string expected = "hello_world";
        var actual = "hello/world".ReplaceIllegalFileNameCharacters();
        Assert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void ReplaceIllegalCharactersTest_MultipleIllegalChars_AreReplaced()
    {
        const string expected = "hello_world_is_this_the_real_life";
        var actual = "hello/world/is/this/the/real/life".ReplaceIllegalFileNameCharacters();
        Assert.AreEqual(expected, actual);
    }
}