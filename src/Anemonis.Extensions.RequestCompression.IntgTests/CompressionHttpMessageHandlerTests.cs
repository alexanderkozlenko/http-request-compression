#pragma warning disable IDE1006

using System.IO.Compression;
using System.Net.Http.Json;
using System.Net.Mime;
using Anemonis.Extensions.RequestCompression.IntgTests.TestStubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.IntgTests;

[TestClass]
public sealed class CompressionHttpMessageHandlerTests
{
    [TestMethod]
    public async Task HandlerWithServiceProviderAndDefaultOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);

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

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task HandlerWithServiceProviderAndSpecificOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var stream = await request.Content.ReadAsStreamAsync();
            var message = AsBrotliEncodedString(stream);

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddRequestCompression(options =>
            {
                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
                options.DefaultEncodingName = "identity";
                options.DefaultCompressionLevel = CompressionLevel.SmallestSize;
                options.DefaultMimeTypes.Clear();
                options.DefaultMimeTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br", CompressionLevel.Optimal, new[] { MediaTypeNames.Application.Json });

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

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

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNull(request.Content.Headers.ContentLength);

            return new();
        }

        var compressionOptions = new RequestCompressionOptions();

        compressionOptions.Providers.Add<BrotliCompressionProvider>();

        var compressionProviderRegistry = new RequestCompressionProviderRegistry(Options.Create(compressionOptions));
        var compressionProvider = compressionProviderRegistry.GetProvider("br");
        var compressionLevel = CompressionLevel.Fastest;
        var mediaTypes = new[] { MediaTypeNames.Application.Json };

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel, mediaTypes)
        {
            InnerHandler = new TestPrimaryHandler(PrimaryHandler),
        };

        var httpClient = new HttpClient(httpMessageHandler);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    private static string AsBrotliEncodedString(Stream inputStream)
    {
        using var decodeStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true);
        using var streamReader = new StreamReader(decodeStream);

        return streamReader.ReadToEnd();
    }
}
