#pragma warning disable CS1998
#pragma warning disable IDE1006

using Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerTests
{
    [TestMethod]
    public void SendWhenContentIsNull()
    {
        static async Task<HttpResponseMessage> BedrockHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNull(request.Content);

            return new();
        }

        var handler = new RequestCompressionHttpMessageHandler(new TestCompressionProvider(), default);
        var adapter = new TestDelegatingHandler(handler, BedrockHandler);
        var request = new HttpRequestMessage();

        adapter.Send(request, default);
    }

    [TestMethod]
    public void SendWhenContentIsNotNull()
    {
        static async Task<HttpResponseMessage> BedrockHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(new TestCompressionProvider(), default);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, BedrockHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        httpMessageHandlerAdapter.Send(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenContentIsNull()
    {
        static async Task<HttpResponseMessage> BedrockHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNull(request.Content);

            return new();
        }

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(new TestCompressionProvider(), default);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, BedrockHandler);
        var httpRequestMessage = new HttpRequestMessage();

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }

    [TestMethod]
    public async Task SendAsyncWhenContentIsNotNull()
    {
        static async Task<HttpResponseMessage> BedrockHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("e1m", message);
            Assert.AreEqual(2, request.Content.Headers.ContentEncoding.Count);
            Assert.AreEqual("e1", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);
            Assert.AreEqual(DateTimeOffset.UnixEpoch, request.Content.Headers.LastModified);

            return new();
        }

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(new TestCompressionProvider(), default);
        var httpMessageHandlerAdapter = new TestDelegatingHandler(httpMessageHandler, BedrockHandler);
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Content = new StringContent("m");
        httpRequestMessage.Content.Headers.ContentEncoding.Add("identity");
        httpRequestMessage.Content.Headers.LastModified = DateTimeOffset.UnixEpoch;

        await httpMessageHandlerAdapter.SendAsync(httpRequestMessage, default);
    }
}
