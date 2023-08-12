#pragma warning disable IDE1006
#pragma warning disable CS1998

using FakeItEasy;
using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("em", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
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

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, false);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEnableSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("em", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, true);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
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

        var compressionProvider = new TestCompressionProvider("a") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("a", out compressionProvider))
            .Returns(true);

        options.EncodingName = "b";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingName, "a");

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndEncodingIsNull()
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
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = null;
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
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
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = "identity";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
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

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
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

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, false);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupportedAndEnableSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("em", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";
        options.CompressionLevel = CompressionLevel.NoCompression;
        options.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.CompressionEnabled, true);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenContextIsNull()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            return new();
        }

        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, null);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsNotSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "c; q=0.9, a; q=0.8, d; q=0.7, b; q=0.6");

            return response;
        }

        var compressionProvider = default(IRequestCompressionProvider);
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider(A<string>.Ignored, out compressionProvider))
            .Returns(false);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("identity", out compressionProvider))
            .Returns(true);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("a", out compressionProvider))
            .Returns(false);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("b", out compressionProvider))
            .Returns(false);

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
        Assert.IsTrue(encodingContext.IsDiscoverySupported);
        Assert.AreEqual(null, encodingContext.EncodingName);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsSupported1()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "e");

            return response;
        }

        var compressionProvider = new TestCompressionProvider("e") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("e", out compressionProvider))
            .Returns(true);

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
        Assert.IsTrue(encodingContext.IsDiscoverySupported);
        Assert.AreEqual("e", encodingContext.EncodingName);
    }

    [TestMethod]
    public async Task SendAsyncWithDiscoveryWhenEncodingIsSupportedN()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            var response = new HttpResponseMessage();

            response.Headers.TryAddWithoutValidation("Accept-Encoding", "c; q=0.9, a; q=0.8, d; q=0.7, b; q=0.6");

            return response;
        }

        var compressionProvider1 = default(IRequestCompressionProvider);
        var compressionProvider2 = new TestCompressionProvider("a") as IRequestCompressionProvider;
        var compressionProvider3 = new TestCompressionProvider("b") as IRequestCompressionProvider;
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider(A<string>.Ignored, out compressionProvider1))
            .Returns(false);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("identity", out compressionProvider1))
            .Returns(true);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("a", out compressionProvider2))
            .Returns(true);
        A.CallTo(() => compressionProviderRegistry.TryGetProvider("b", out compressionProvider3))
            .Returns(true);

        options.EncodingName = "identity";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
        Assert.IsTrue(encodingContext.IsDiscoverySupported);
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
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("identity", out compressionProvider))
            .Returns(true);

        options.EncodingName = "e";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
        Assert.IsTrue(encodingContext.IsDiscoverySupported);
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
        var compressionProviderRegistry = A.Fake<IRequestCompressionProviderRegistry>(x => x.Strict());
        var logger = NullLogger.Instance;
        var options = new RequestCompressionHttpMessageHandlerOptions();

        A.CallTo(() => compressionProviderRegistry.TryGetProvider("*", out compressionProvider))
            .Returns(false);

        options.EncodingName = "e";

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, options, logger);
        var httpMessageHandlerAdapter = new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();
        var encodingContext = new RequestCompressionEncodingContext();

        httpRequestMessage.Method = HttpMethod.Options;
        httpRequestMessage.Options.Set(RequestCompressionOptionKeys.EncodingContext, encodingContext);

        var httpResponseMessage = await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);

        Assert.IsNotNull(httpResponseMessage);
        Assert.IsTrue(encodingContext.IsDiscoverySupported);
        Assert.AreEqual("e", encodingContext.EncodingName);
    }

    private sealed class TestCompressionProvider : IRequestCompressionProvider
    {
        public TestCompressionProvider(string encodingName)
        {
            EncodingName = encodingName;
        }

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
            get;
        }
    }
}
