using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionProviderRegistryTests
{
    [TestMethod]
    public void GetProvider()
    {
        var options = new RequestCompressionOptions();

        options.Providers.Add<TestCompressionProvider>();

        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));
        var provider = providerRegistry.GetProvider("e1");

        Assert.IsNotNull(provider);
        Assert.IsInstanceOfType(provider, typeof(TestCompressionProvider));
    }

    [TestMethod]
    public void GetProviderWhenNotRegistered()
    {
        var options = new RequestCompressionOptions();
        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));

        Assert.ThrowsException<InvalidOperationException>(() =>
            providerRegistry.GetProvider("e1"));
    }
}
