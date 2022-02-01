using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerFactoryTests
{
    [TestMethod]
    public void CreateHandler()
    {
        var options = new RequestCompressionOptions();

        options.DefaultMediaTypes.Clear();
        options.Providers.Add<TestCompressionProvider>();

        var provider = new Mock<IRequestCompressionProvider>(MockBehavior.Strict);
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);

        providerRegistry
            .Setup(o => o.GetProvider(It.IsAny<string>()))
            .Returns(provider.Object);

        var loggerFactory = NullLoggerFactory.Instance;
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry.Object, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("e1", CompressionLevel.Optimal, new[] { "text/plain" });

        Assert.IsNotNull(httpMessageHandler);
    }

    [TestMethod]
    public void CreateHandlerFromOptions()
    {
        var options = new RequestCompressionOptions();

        options.DefaultEncodingName = "e1";
        options.DefaultCompressionLevel = CompressionLevel.Optimal;
        options.DefaultMediaTypes.Clear();
        options.DefaultMediaTypes.Add("text/plain");
        options.Providers.Add<TestCompressionProvider>();

        var provider = new Mock<IRequestCompressionProvider>(MockBehavior.Strict);
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);

        providerRegistry
            .Setup(o => o.GetProvider(It.IsAny<string>()))
            .Returns(provider.Object);

        var loggerFactory = NullLoggerFactory.Instance;
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry.Object, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler(null, null, null);

        Assert.IsNotNull(httpMessageHandler);
    }
}
