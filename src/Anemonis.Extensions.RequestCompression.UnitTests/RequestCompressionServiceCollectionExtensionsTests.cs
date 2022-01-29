using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddRequestCompressionWhenServicesIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionServiceCollectionExtensions.AddRequestCompression(null!));
    }

    [TestMethod]
    public void AddRequestCompression()
    {
        var builder = new Mock<IServiceCollection>(MockBehavior.Loose);
        var result = RequestCompressionServiceCollectionExtensions.AddRequestCompression(builder.Object);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void AddRequestCompressionWithActionWhenServicesIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionServiceCollectionExtensions.AddRequestCompression(null!, o => { }));
    }

    [TestMethod]
    public void AddRequestCompressionWithActionWhenActionIsNull()
    {
        var services = new ServiceCollection();

        Assert.ThrowsException<ArgumentNullException>(() =>
            RequestCompressionServiceCollectionExtensions.AddRequestCompression(services, null!));
    }

    [TestMethod]
    public void AddRequestCompressionWithAction()
    {
        var builder = new Mock<IServiceCollection>(MockBehavior.Loose);
        var result = RequestCompressionServiceCollectionExtensions.AddRequestCompression(builder.Object, o => { });

        Assert.IsNotNull(result);
    }
}
