using System.IO.Compression;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerFactoryTests
{
    [TestMethod]
    public void CreateHandlerFromParameters()
    {
        var options = new RequestCompressionOptions();
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
        options.DefaultMediaTypes.Add("text/plain");

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

    [TestMethod]
    public void CreateHandlerWhenEncodingNameIsNotDefined()
    {
        var options = new RequestCompressionOptions();
        var provider = new Mock<IRequestCompressionProvider>(MockBehavior.Strict);
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var loggerFactory = NullLoggerFactory.Instance;
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry.Object, loggerFactory);

        Assert.ThrowsException<InvalidOperationException>(() =>
            httpMessageHandlerFactory.CreateHandler(null, CompressionLevel.Optimal, new[] { "text/plain" }));
    }

    [TestMethod]
    public void CreateHandlerWhenCompressionLevelIsNotDefined()
    {
        var options = new RequestCompressionOptions();
        var provider = new Mock<IRequestCompressionProvider>(MockBehavior.Strict);
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var loggerFactory = NullLoggerFactory.Instance;
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry.Object, loggerFactory);

        Assert.ThrowsException<InvalidOperationException>(() =>
            httpMessageHandlerFactory.CreateHandler("e1", null, new[] { "text/plain" }));
    }

    [TestMethod]
    public void CreateHandlerWhenMediaTypesIsNotDefined()
    {
        var options = new RequestCompressionOptions();
        var provider = new Mock<IRequestCompressionProvider>(MockBehavior.Strict);
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);

        providerRegistry
            .Setup(o => o.GetProvider(It.IsAny<string>()))
            .Returns(provider.Object);

        var loggerFactory = NullLoggerFactory.Instance;
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry.Object, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("e1", CompressionLevel.Optimal, null);

        Assert.IsNotNull(httpMessageHandler);
    }
}
