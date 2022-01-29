using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class GzipCompressionProviderTests
{
    [TestMethod]
    public void GetEncodingName()
    {
        var provider = new GzipCompressionProvider();

        Assert.AreEqual("gzip", provider.EncodingName, ignoreCase: true);
    }

    [TestMethod]
    public void CreateStream()
    {
        var provider = new GzipCompressionProvider();
        var inputStream = Stream.Null;
        var outputStream = provider.CreateStream(inputStream, CompressionLevel.NoCompression);

        Assert.IsNotNull(outputStream);
    }
}
