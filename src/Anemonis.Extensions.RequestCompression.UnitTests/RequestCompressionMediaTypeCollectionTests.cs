using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionMediaTypeCollectionTests
{
    [TestMethod]
    public void AddWhenItemIsNull()
    {
        var collection = new RequestCompressionMediaTypeCollection();

        Assert.ThrowsException<ArgumentNullException>(() =>
            collection.Add(null!));
    }

    [TestMethod]
    public void AddWhenCalledNTimesWithCasing()
    {
        var collection = new RequestCompressionMediaTypeCollection();

        collection.Add("application/json");
        collection.Add("application/JSON");

        Assert.AreEqual(1, collection.Count);
        Assert.IsTrue(collection.Contains("application/json"));
    }
}
