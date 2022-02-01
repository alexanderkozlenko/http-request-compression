using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionOptionsTests
{
    [TestMethod]
    public void Constructor()
    {
        var options = new RequestCompressionOptions();

        Assert.IsNotNull(options.Providers);
        Assert.AreEqual(0, options.Providers.Count);
        Assert.IsNotNull(options.DefaultMediaTypes);
        Assert.AreEqual(0, options.DefaultMediaTypes.Count);
    }
}
