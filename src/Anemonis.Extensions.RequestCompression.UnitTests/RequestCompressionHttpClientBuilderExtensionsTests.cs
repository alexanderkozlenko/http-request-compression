using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpClientBuilderExtensionsTests
{
    [TestMethod]
    public void AddRequestCompressionHandlerWhenBuilderIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionHttpClientBuilderExtensions.AddRequestCompressionHandler(null!, "e", default));
    }

    [TestMethod]
    public void AddRequestCompressionHandlerWhenEncodingNameIsNull()
    {
        var builder = new Mock<IHttpClientBuilder>(MockBehavior.Loose);

        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionHttpClientBuilderExtensions.AddRequestCompressionHandler(builder.Object, null!, default));
    }

    [TestMethod]
    public void AddRequestCompressionHandler()
    {
        var builder = new Mock<IHttpClientBuilder>(MockBehavior.Loose);
        var services = new Mock<IServiceCollection>(MockBehavior.Loose);

        builder
            .Setup(o => o.Services)
            .Returns(services.Object);

        var result = RequestCompressionHttpClientBuilderExtensions.AddRequestCompressionHandler(builder.Object, "e", default);

        Assert.IsNotNull(result);
    }
}
