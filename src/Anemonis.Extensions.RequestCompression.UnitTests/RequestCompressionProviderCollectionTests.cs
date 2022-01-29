using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionProviderCollectionTests
{
    [TestMethod]
    public void AddAsGenericParameter()
    {
        var collection = new RequestCompressionProviderCollection();

        collection.Add<TestCompressionProvider>();

        Assert.AreEqual(1, collection.Count);
        Assert.IsTrue(collection.Contains(typeof(TestCompressionProvider)));
    }

    [TestMethod]
    public void AddAsGenericParameterWhenCalledNTimes()
    {
        var collection = new RequestCompressionProviderCollection();

        collection.Add<TestCompressionProvider>();
        collection.Add<TestCompressionProvider>();

        Assert.AreEqual(1, collection.Count);
        Assert.IsTrue(collection.Contains(typeof(TestCompressionProvider)));
    }

    [TestMethod]
    public void AddAsTypeObjectWhenTypeDoesNotImplementInterface()
    {
        var collection = new RequestCompressionProviderCollection();

        Assert.ThrowsException<ArgumentException>(() =>
            collection.Add(typeof(DeflateStream)));
    }

    [TestMethod]
    public void AddAsTypeObject()
    {
        var collection = new RequestCompressionProviderCollection();

        collection.Add(typeof(TestCompressionProvider));

        Assert.AreEqual(1, collection.Count);
        Assert.IsTrue(collection.Contains(typeof(TestCompressionProvider)));
    }

    [TestMethod]
    public void AddAsTypeObjectWhenCalledNTimes()
    {
        var collection = new RequestCompressionProviderCollection();

        collection.Add(typeof(TestCompressionProvider));
        collection.Add(typeof(TestCompressionProvider));

        Assert.AreEqual(1, collection.Count);
        Assert.IsTrue(collection.Contains(typeof(TestCompressionProvider)));
    }
}
