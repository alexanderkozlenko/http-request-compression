#pragma warning disable IDE1006

using System.IO.Compression;
using System.Net.Http.Json;
using System.Net.Mime;
using Anemonis.Extensions.RequestCompression.IntgTests.TestStubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.IntgTests;

[TestClass]
public sealed class CompressionHttpMessageHandlerTests
{
    [TestMethod]
    public async Task CompressWithServiceProviderAndDefaultOptions()
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
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task SkipWithServiceProviderAndDefaultOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello World!", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task ForcedSkipWithServiceProviderAndDefaultOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");
        httpRequestMessage.SetCompressionEnabled(false);

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task CompressWithServiceProviderAndSpecificOptions()
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
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression(options =>
            {
                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
                options.DefaultEncodingName = "identity";
                options.DefaultCompressionLevel = CompressionLevel.SmallestSize;
                options.DefaultMediaTypes.Clear();
                options.DefaultMediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br", CompressionLevel.Optimal, new[] { MediaTypeNames.Application.Json });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task SkipWithServiceProviderAndSpecificOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello World!", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression(options =>
            {
                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
                options.DefaultEncodingName = "identity";
                options.DefaultCompressionLevel = CompressionLevel.SmallestSize;
                options.DefaultMediaTypes.Clear();
                options.DefaultMediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br", CompressionLevel.Optimal, new[] { MediaTypeNames.Application.Json });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task ForcedSkipWithServiceProviderAndSpecificOptions()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression(options =>
            {
                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
                options.DefaultEncodingName = "identity";
                options.DefaultCompressionLevel = CompressionLevel.SmallestSize;
                options.DefaultMediaTypes.Clear();
                options.DefaultMediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler("br", CompressionLevel.Optimal, new[] { MediaTypeNames.Application.Json });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");
        httpRequestMessage.SetCompressionEnabled(false);

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task CompressWithoutServiceProvider()
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

        using var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole());

        var options = new RequestCompressionOptions();

        options.Providers.Add<BrotliCompressionProvider>();

        var compressionLevel = CompressionLevel.Fastest;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { MediaTypeNames.Application.Json });
        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("br", compressionLevel, mediaTypes);

        httpMessageHandler.InnerHandler = new TestPrimaryHandler(PrimaryHandler);

        var httpClient = new HttpClient(httpMessageHandler);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task SkipWithoutServiceProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello World!", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        using var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole());

        var logger = loggerFactory.CreateLogger<RequestCompressionHttpMessageHandler>();
        var options = new RequestCompressionOptions();

        options.Providers.Add<BrotliCompressionProvider>();

        var compressionLevel = CompressionLevel.Fastest;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { MediaTypeNames.Application.Json });
        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("br", compressionLevel, mediaTypes);

        httpMessageHandler.InnerHandler = new TestPrimaryHandler(PrimaryHandler);

        var httpClient = new HttpClient(httpMessageHandler);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task ForcedSkipWithoutServiceProvider()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Content);

            var message = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("\"Hello World!\"", message);
            Assert.IsNull(request.Content.Headers.ContentEncoding.LastOrDefault());
            Assert.IsNotNull(request.Content.Headers.ContentLength);

            return new();
        }

        using var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole());

        var logger = loggerFactory.CreateLogger<RequestCompressionHttpMessageHandler>();
        var options = new RequestCompressionOptions();

        options.Providers.Add<BrotliCompressionProvider>();

        var compressionLevel = CompressionLevel.Fastest;
        var mediaTypes = new RequestCompressionMediaTypeCollection(new[] { MediaTypeNames.Application.Json });
        var providerRegistry = new RequestCompressionProviderRegistry(Options.Create(options));
        var httpMessageHandlerFactory = new RequestCompressionHttpMessageHandlerFactory(Options.Create(options), providerRegistry, loggerFactory);
        var httpMessageHandler = httpMessageHandlerFactory.CreateHandler("br", compressionLevel, mediaTypes);

        httpMessageHandler.InnerHandler = new TestPrimaryHandler(PrimaryHandler);

        var httpClient = new HttpClient(httpMessageHandler);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");
        httpRequestMessage.SetCompressionEnabled(false);

        await httpClient.SendAsync(httpRequestMessage);
    }

    private static string AsBrotliEncodedString(Stream inputStream)
    {
        using var decodeStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true);
        using var streamReader = new StreamReader(decodeStream);

        return streamReader.ReadToEnd();
    }
}
