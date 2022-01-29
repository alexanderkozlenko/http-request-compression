using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class BrotliCompressionProviderTests
{
    [TestMethod]
    public void GetEncodingName()
    {
        var provider = new BrotliCompressionProvider();

        Assert.AreEqual("br", provider.EncodingName, ignoreCase: true);
    }

    [TestMethod]
    public void CreateStream()
    {
        var provider = new BrotliCompressionProvider();
        var inputStream = Stream.Null;
        var outputStream = provider.CreateStream(inputStream, CompressionLevel.NoCompression);

        Assert.IsNotNull(outputStream);
    }
}
