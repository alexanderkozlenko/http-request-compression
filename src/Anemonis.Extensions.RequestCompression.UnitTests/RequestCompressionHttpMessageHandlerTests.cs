#pragma warning disable CS1998

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerTests
{
    [TestMethod]
    public async Task SendAsyncWhenContentIsNull()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNull(request.Content);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("am", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("a", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEnableSwitchIsFalse()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("m", message);
            Assert.AreEqual(1, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("identity", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, false);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEnableSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("am", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("a", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, true);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEncodingSwitchIsSet()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("am", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("a", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "b";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingName, "a");

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEncodingIsNully()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("m", message);
            Assert.AreEqual(1, request.Content.Headers.ContentEncoding.Count);
            Assert.IsNotNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = default(IRequestCompressionProvider);
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = null;
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEncodingIsIdentity()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("m", message);
            Assert.AreEqual(1, request.Content.Headers.ContentEncoding.Count);
            Assert.IsNotNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = default(IRequestCompressionProvider);
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = "identity";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("m", message);
            Assert.AreEqual(1, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("identity", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupportedAndEnableSwitchIsFalse()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("m", message);
            Assert.AreEqual(1, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("identity", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, false);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupportedAndEnableSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("am", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("a", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, true);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsNotSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "c, a; q=0.9, d; q=0.9, b; q=0.8");

            return response;
        }

        var compressionProvider0 = default(IRequestCompressionProvider);
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider(It.IsAny<string>(), out compressionProvider0))
            .Returns(false);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("identity", out compressionProvider0))
            .Returns(true);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider0))
            .Returns(false);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("b", out compressionProvider0))
            .Returns(false);

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequerstCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.AreEqual(null, encodingContext.EncodingName);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "c, a; q=0.9, d; q=0.9, b; q=0.8");

            return response;
        }

        var compressionProvider0 = default(IRequestCompressionProvider);
        var compressionProvider1 = new TestCompressionProvider() as IRequestCompressionProvider;
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider(It.IsAny<string>(), out compressionProvider0))
            .Returns(false);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("identity", out compressionProvider0))
            .Returns(true);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("a", out compressionProvider1))
            .Returns(true);
        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("b", out compressionProvider1))
            .Returns(true);

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequerstCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.AreEqual("a", encodingContext.EncodingName);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsIdentity()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");

            return response;
        }

        var compressionProvider = default(IRequestCompressionProvider);
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = "a";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequerstCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.AreEqual(null, encodingContext.EncodingName);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsAny()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "*");

            return response;
        }

        var compressionProvider = default(IRequestCompressionProvider);
        var compressionProviderRegistry = new Mock<IRequestCompressionProviderRegistry>(MockBehavior.Strict);
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        compressionProviderRegistry
            .Setup(o => o.TryGetProvider("*", out compressionProvider))
            .Returns(false);

        options.EncodingName = "a";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry.Object, logger, options);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequerstCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.AreEqual("a", encodingContext.EncodingName);
    }

    private sealed class TestCompressionProvider : IRequestCompressionProvider
    {
        public Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
        {
            var inputStream = new DelegatingStream(outputStream);

            using var streamWriter = new StreamWriter(inputStream);

            streamWriter.Write(EncodingName);
            streamWriter.Flush();

            return inputStream;
        }

        public string EncodingName
        {
            get
            {
                return "a";
            }
        }
    }
}
