#pragma warning disable CS1998
#pragma warning disable IDE1006

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerTests
{
    [TestMethod]
    public void SendWhenContentIsNull()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNull(request.Content);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection();
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsSupported()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsSupportedAndSwitchIsFalse()
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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, false);

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsSupportedAndSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, true);

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsNotSupported()
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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsNotSupportedAndSwitchIsFalse()
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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, false);

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenMediaTypeIsNotSupportedAndSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, true);

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenContentIsNull()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNull(request.Content);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection();
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
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

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndSwitchIsFalse()
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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, false);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsSupportedAndSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "text/plain" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, true);

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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupportedAndSwitchIsFalse()
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

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, false);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenMediaTypeIsNotSupportedAndSwitchIsTrue()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var compressionProvider = new TestCompressionProvider();
        var compressionLevel = CompressionLevel.Optimal;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { "application/json" });
        var logger = NullLogger.Instance;
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;
        httpRequestMessage.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionKey, true);

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }
}
