using System.IO.Compression;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionProviderRegistryTests
{
    [TestMethod]
    public void GetProviderWhenNameNotRegistered()
    {
        var options = new RequestCompressionOptions();

        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));

        var result = providerRegistry.TryGetProvider("a", out var provider);

        Assert.IsFalse(result);
        Assert.IsNull(provider);
    }

    [TestMethod]
    public void GetProvider()
    {
        var options = new RequestCompressionOptions();

        options.Providers.Add<TestCompressionProvider>();

        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));

        var result = providerRegistry.TryGetProvider("e", out var provider);

        Assert.IsTrue(result);
        Assert.IsNotNull(provider);
    }

    [TestMethod]
    public void GetProviderWhenNameIsIdentity()
    {
        var options = new RequestCompressionOptions();

        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));

        var result = providerRegistry.TryGetProvider("identity", out var provider);

        Assert.IsTrue(result);
        Assert.IsNull(provider);
    }

    private sealed class TestCompressionProvider : IRequestCompressionProvider
    {
        public Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
        {
            throw new NotSupportedException();
        }

        public string EncodingName
        {
            get
            {
                return "e";
            }
        }
    }
}
