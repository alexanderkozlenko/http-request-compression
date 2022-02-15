using System.IO.Compression;
using System.Net.Http.Json;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.IntgTests;

[TestClass]
public sealed class RequestCompressionHttpMessageHandlerTests
{
    [TestMethod]
    public async Task ApplyCompressionWithDefaultOptions()
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
    public async Task SkipCompressionWithDefaultOptions()
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
    public async Task SkipCompressionWithDefaultOptionsByRequestSwitch()
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
        httpRequestMessage.Options.SetCompressionEnabled(false);

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task ApplyCompressionWithSpecificOptions()
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
                options.EncodingName = "identity";
                options.CompressionLevel = CompressionLevel.SmallestSize;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler(options =>
            {
                options.EncodingName = "br";
                options.CompressionLevel = CompressionLevel.Optimal;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Json);
            });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task SkipCompressionWithSpecificOptions()
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
                options.EncodingName = "identity";
                options.CompressionLevel = CompressionLevel.SmallestSize;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler(options =>
            {
                options.EncodingName = "br";
                options.CompressionLevel = CompressionLevel.Optimal;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Json);
            });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = new StringContent("Hello World!");

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task SkipCompressionWithSpecificOptionsByRequestSwitch()
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
                options.EncodingName = "identity";
                options.CompressionLevel = CompressionLevel.SmallestSize;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Octet);
            });

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler(options =>
            {
                options.EncodingName = "br";
                options.CompressionLevel = CompressionLevel.Optimal;
                options.MediaTypes.Clear();
                options.MediaTypes.Add(MediaTypeNames.Application.Json);
            });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage.Content = JsonContent.Create("Hello World!");
        httpRequestMessage.Options.SetCompressionEnabled(false);

        await httpClient.SendAsync(httpRequestMessage);
    }

    [TestMethod]
    public async Task UseDiscoveredCompressionFormat()
    {
        static async Task<HttpResponseMessage> PrimaryHandler(HttpRequestMessage request)
        {
            Assert.IsNotNull(request);

            if (request.Method == HttpMethod.Options)
            {
                var response = new HttpResponseMessage();

                response.Headers.TryAddWithoutValidation("Accept-Encoding", "br;q=1.0, identity; q=0.5, *;q=0");

                return response;
            }
            else
            {
                Assert.IsNotNull(request.Content);

                var stream = await request.Content.ReadAsStreamAsync();
                var message = AsBrotliEncodedString(stream);

                Assert.AreEqual("\"Hello World!\"", message);
                Assert.AreEqual("br", request.Content.Headers.ContentEncoding.LastOrDefault());
                Assert.IsNull(request.Content.Headers.ContentLength);

                return new();
            }
        }

        var serviceCollection = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .AddRequestCompression();

        serviceCollection
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new TestPrimaryHandler(PrimaryHandler))
            .AddRequestCompressionHandler(options =>
            {
                options.EncodingName = "identity";
            });

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var httpRequestMessage1 = new HttpRequestMessage(HttpMethod.Options, "http://localhost");

        httpRequestMessage1.Options.AddCompressionDiscovery(out var encodingContext);

        await httpClient.SendAsync(httpRequestMessage1);

        Assert.AreEqual("br", encodingContext.EncodingName);

        var httpRequestMessage2 = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        httpRequestMessage2.Content = JsonContent.Create("Hello World!");
        httpRequestMessage2.Options.SetCompressionEncoding(encodingContext.EncodingName);

        await httpClient.SendAsync(httpRequestMessage2);
    }

    private static string AsBrotliEncodedString(Stream inputStream)
    {
        using var decodeStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true);
        using var streamReader = new StreamReader(decodeStream);

        return streamReader.ReadToEnd();
    }

    private sealed class TestPrimaryHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestPrimaryHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler.Invoke(request).GetAwaiter().GetResult();
        }

        protected sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler.Invoke(request);
        }
    }
}
