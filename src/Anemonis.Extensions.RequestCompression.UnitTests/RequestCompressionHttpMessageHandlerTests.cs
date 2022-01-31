#pragma warning disable CS1998
#pragma warning disable IDE1006

using System.IO.Compression;
using System.Text;
using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.Extensions.Logging;
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
        var mediaTypes = Array.Empty<string>();
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenContentIsNotNull()
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
        var mediaTypes = new[] { "text/plain" };
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m", Encoding.UTF8, null);
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public void SendWhenContentIsNotNullAndMediaTypeIsExcluded()
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
        var mediaTypes = new[] { "application/json" };
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m", Encoding.UTF8, null);
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

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
        var mediaTypes = Array.Empty<string>();
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenContentIsNotNull()
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
        var mediaTypes = new[] { "text/plain" };
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenContentIsNotNullAndMediaTypeIsExcluded()
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
        var mediaTypes = new[] { "application/json" };
        var logger = default(ILogger);
        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes, logger);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, PrimaryHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }
}
