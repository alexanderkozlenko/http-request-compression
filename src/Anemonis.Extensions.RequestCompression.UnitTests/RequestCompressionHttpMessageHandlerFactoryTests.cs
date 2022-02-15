using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerFactoryTests
{
    [TestMethod]
    public void CreateHandlerWhenNameIsNull()
    {
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = new Mock<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(MockBehavior.Strict);

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry.Object, loggerFactory, optionsMonitor.Object);

        Assert.ThrowsException<ArgumentNullException>(() =>
            httpMessageHandlerFactory.CreateHandler(null!));
    }

    [TestMethod]
    public void CreateHandlerWhenInstanceIsDefault()
    {
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = new Mock<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(MockBehavior.Strict);
        var options = new RequestCompressionHttpMessageHandlerOptions();

        optionsMonitor
            .Setup(o => o.Get(Options.DefaultName))
            .Returns(options);

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry.Object, loggerFactory, optionsMonitor.Object);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler(Options.DefaultName);

        Assert.IsNotNull(httpMessageHandler);
    }

    [TestMethod]
    public void CreateHandlerWhenInstanceIsNamed()
    {
        var providerRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = new Mock<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(MockBehavior.Strict);
        var options = new RequestCompressionHttpMessageHandlerOptions();

        optionsMonitor
            .Setup(o => o.Get("name"))
            .Returns(options);

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry.Object, loggerFactory, optionsMonitor.Object);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("name");

        Assert.IsNotNull(httpMessageHandler);
    }
}
