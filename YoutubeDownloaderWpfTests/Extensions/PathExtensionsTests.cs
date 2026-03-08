using YoutubeDownloader.Core.Extensions;

namespace YoutubeDownloaderWpfTests.Extensions;

[TestClass]
public class PathExtensionsTests
{
    [TestMethod]
    public void ReplaceIllegalCharactersTest_NoIllegalChars_IsIdentical()
    {
        const string expected = "hello world";
        var actual = expected.ReplaceIllegalCharacters();
        Assert.AreEqual(expected, actual);
    }
}