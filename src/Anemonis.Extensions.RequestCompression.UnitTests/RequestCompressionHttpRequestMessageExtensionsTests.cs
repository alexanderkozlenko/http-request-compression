using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpRequestMessageExtensionsTests
{
    [TestMethod]
    public void SetCompressionEnabledIfRequestIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionHttpRequestMessageExtensions.SetCompressionEnabled(null!, false));
    }
}
