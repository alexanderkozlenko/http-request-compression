using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerFactoryTests
{
    [TestMethod]
    public void CreateHandlerWhenNameIsNull()
    {
        var providerRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = A.Fake<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(x => x.Strict());

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry, optionsMonitor, loggerFactory);

        Assert.ThrowsException<ArgumentNullException>(() =>
            httpMessageHandlerFactory.CreateHandler(null!));
    }

    [TestMethod]
    public void CreateHandlerWhenInstanceIsDefault()
    {
        var providerRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = A.Fake<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(x => x.Strict());
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => optionsMonitor.Get(Options.DefaultName))
            .Returns(options);

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry, optionsMonitor, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler(Options.DefaultName);

        Assert.IsNotNull(httpMessageHandler);
    }

    [TestMethod]
    public void CreateHandlerWhenInstanceIsNamed()
    {
        var providerRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var loggerFactory = NullLoggerFactory.Instance;
        var optionsMonitor = A.Fake<IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions>>(x => x.Strict());
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => optionsMonitor.Get("name"))
            .Returns(options);

        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(providerRegistry, optionsMonitor, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("name");

        Assert.IsNotNull(httpMessageHandler);
    }
}
