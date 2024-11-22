using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

[TestClass]
public sealed partial class HttpCompressionHandlerTests
{
    [TestMethod]
    public async Task AddCompressionWithDefaultOptions()
    {
        static Task<HttpResponseMessage> HandleHttpRequest(HttpRequestMessage request)
        {
            Assert.IsNotNull(request.Content);
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual("br", string.Join('+', request.Content.Headers.ContentEncoding));

            return Task.FromResult(new HttpResponseMessage());
        }

        var services = new ServiceCollection();

        services
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new FakeMessageHandler(HandleHttpRequest))
            .AddCompressionHandler();

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<HttpClient>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        request.Content = new StringContent("content");
        request.Content.Headers.ContentType = new("application/json");

        using var response = await client.SendAsync(request);
    }

    [TestMethod]
    public async Task AddCompressionWithGlobalOptions()
    {
        static async Task<HttpResponseMessage> HandleHttpRequest(HttpRequestMessage request)
        {
            Assert.IsNotNull(request.Content);
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual("fe", string.Join('+', request.Content.Headers.ContentEncoding));

            var content = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("content+fe", content);

            return new();
        }

        var services = new ServiceCollection();

        services
            .Configure<HttpCompressionOptions>(options =>
            {
                options.CompressionProviders.Clear();
                options.CompressionProviders["fe"] = new FakeCompressionProvider("fe");
                options.MediaTypes.Clear();
                options.MediaTypes.Add("text/plain");
                options.CompressionEncoding = "fe";
            })
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new FakeMessageHandler(HandleHttpRequest))
            .AddCompressionHandler();

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<HttpClient>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        request.Content = new StringContent("content");

        using var response = await client.SendAsync(request);
    }

    [TestMethod]
    public async Task AddCompressionWithLocalOptions()
    {
        static async Task<HttpResponseMessage> HandleHttpRequest(HttpRequestMessage request)
        {
            Assert.IsNotNull(request.Content);
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual("fe", string.Join('+', request.Content.Headers.ContentEncoding));

            var content = await request.Content.ReadAsStringAsync();

            Assert.AreEqual("content+fe", content);

            return new();
        }

        var services = new ServiceCollection();

        services
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new FakeMessageHandler(HandleHttpRequest))
            .AddCompressionHandler(options =>
            {
                options.CompressionProviders.Clear();
                options.CompressionProviders["fe"] = new FakeCompressionProvider("fe");
                options.MediaTypes.Clear();
                options.MediaTypes.Add("text/plain");
                options.CompressionEncoding = "fe";
            });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<HttpClient>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        request.Content = new StringContent("content");

        using var response = await client.SendAsync(request);
    }

    [TestMethod]
    public async Task AddCompressionChain()
    {
        static Task<HttpResponseMessage> HandleHttpRequest(HttpRequestMessage request)
        {
            Assert.IsNotNull(request.Content);
            Assert.IsNull(request.Content.Headers.ContentLength);
            Assert.AreEqual("fe1+fe2", string.Join('+', request.Content.Headers.ContentEncoding));

            return Task.FromResult(new HttpResponseMessage());
        }

        var services = new ServiceCollection();

        services
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new FakeMessageHandler(HandleHttpRequest))
            .AddCompressionHandler(options =>
            {
                options.CompressionProviders.Clear();
                options.CompressionProviders["fe1"] = new FakeCompressionProvider("fe1");
                options.MediaTypes.Clear();
                options.MediaTypes.Add("text/plain");
                options.CompressionEncoding = "fe1";
            })
            .AddCompressionHandler(options =>
            {
                options.CompressionProviders.Clear();
                options.CompressionProviders["fe2"] = new FakeCompressionProvider("fe2");
                options.MediaTypes.Clear();
                options.MediaTypes.Add("text/plain");
                options.CompressionEncoding = "fe2";
            });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<HttpClient>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        request.Content = new StringContent("content");

        using var response = await client.SendAsync(request);
    }

    [TestMethod]
    [DataRow("fe", "fe")]
    [DataRow("ue", null)]
    [DataRow("*", null)]
    [DataRow("identity", null)]
    [DataRow("identity, ue, fe, *", "fe")]
    [DataRow("identity;q=1.0, ue;q=0.9, fe;q=0.8, *;q=0.7", "fe")]
    public async Task DiscoverQualifiedEncoding(string serverHeader, string clientHeader)
    {
        Task<HttpResponseMessage> HandleHttpRequest(HttpRequestMessage request)
        {
            if (request.Method.Equals(HttpMethod.Options))
            {
                var response = new HttpResponseMessage();

                response.Headers.Add("Accept-Encoding", serverHeader);

                return Task.FromResult(response);
            }
            else
            {
                Assert.IsNotNull(request.Content);

                if (clientHeader is not null)
                {
                    Assert.IsNull(request.Content.Headers.ContentLength);
                    Assert.AreEqual(clientHeader, string.Join('+', request.Content.Headers.ContentEncoding));
                }

                return Task.FromResult(new HttpResponseMessage());
            }
        }

        var services = new ServiceCollection();

        services
            .Configure<HttpCompressionOptions>(options =>
            {
                options.CompressionProviders.Clear();
                options.CompressionProviders["de"] = new FakeCompressionProvider("de");
                options.CompressionProviders["fe"] = new FakeCompressionProvider("fe");
                options.CompressionEncoding = "de";
                options.MediaTypes.Clear();
                options.MediaTypes.Add("text/plain");
            })
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(() => new FakeMessageHandler(HandleHttpRequest))
            .AddCompressionHandler();

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<HttpClient>();
        var context = new HttpCompressionContext();

        using var request1 = new HttpRequestMessage(HttpMethod.Options, "http://localhost");

        request1.Options.Set(HttpCompressionOptionKeys.HttpCompressionContext, context);

        using var response1 = await client.SendAsync(request1);

        Assert.AreEqual(clientHeader, context.CompressionEncoding);

        using var request2 = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

        request2.Content = new StringContent("content");
        request2.Options.Set(HttpCompressionOptionKeys.HttpCompressionContext, context);

        using var response2 = await client.SendAsync(request2);
    }
}
