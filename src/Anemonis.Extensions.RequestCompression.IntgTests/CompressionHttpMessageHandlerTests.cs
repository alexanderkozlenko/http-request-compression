#pragma warning disable IDE1006

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression.IntgTests.TestStubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.IntgTests;

[TestClass]
public sealed class CompressionHttpMessageHandlerTests
{
    [TestMethod]
    public async Task HandlerWithDefaultRegistrationsAndDefaultProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("Hello World!", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task HandlerWithDefaultRegistrationsAndSpecificProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("Hello World!", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br");

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task HandlerWithSpecificRegistrationsAndDefaultProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("Hello World!", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddRequestCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task HandlerWithSpecificRegistrationsAndSpecificProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("Hello World!", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddRequestCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br", CompressionLevel.SmallestSize);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task HandlerWithoutServiceProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("Hello World!", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.AreEqual(null, request.Content.Headers.ContentLength);

            return new();
        }

        var compressionOptions = new RequestCompressionOptions();

        compressionOptions.Providers.Add<BrotliCompressionProvider>();

        var compressionProviderRegistry = new RequestCompressionProviderRegistry(Options.Create(compressionOptions));
        var compressionProvider = compressionProviderRegistry.GetProvider("br");
        var compressionLevel = CompressionLevel.Fastest;

        var httpHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel)
        {
            InnerHandler = new TestPrimaryHandler(PrimaryHandler),
        };

        var httpClient = new HttpClient(httpHandler);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    private static string AsBrotliEncodedString(Stream inputStream)
    {
        using var decodeStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true);
        using var streamReader = new StreamReader(decodeStream);

        return streamReader.ReadToEnd();
    }
}
