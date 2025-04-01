using Microsoft.VisualStudio.TestTools.UnitTesting;
using YoutubeDownloader.Wpf.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Core.Util;

namespace YoutubeDownloaderWpf.Util.Tests;

[TestClass()]
public class UrlUtilTests
{
    [TestMethod()]
    public void CombineTest_OneArgument_IsIdentical()
    {
        string expected = "hello";
        string actual = UrlUtil.Combine(expected);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_OneArgument_Trailing_IsIdentical()
    {
        string expected = "hello/";
        string actual = UrlUtil.Combine(expected);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_OneArgument_Leading_IsIdentical()
    {
        string expected = "/hello";
        string actual = UrlUtil.Combine(expected);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_TwoArguments_NoSlashes_Equals()
    {
        string expected = "hello/world";
        string actual = UrlUtil.Combine("hello", "world");
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_TwoArguments_TrailingSlashes_Equals()
    {
        string expected = "hello/world";
        string actual = UrlUtil.Combine("hello/", "/world");
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_VariableArguments_NoSlashes_Equals()
    {
        string expected = "hello/world/foo/bar";
        string actual = UrlUtil.Combine("hello", "world", "foo", "bar");
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void CombineTest_VariableArguments_TrailingSlashes_Equals()
    {
        string expected = "hello/world/foo/bar";
        string actual = UrlUtil.Combine("hello////", "/world/", "foo", "bar");
        Assert.AreEqual(expected, actual);
    }
}