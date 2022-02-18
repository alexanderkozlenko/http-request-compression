#pragma warning disable CA1822
#pragma warning disable IDE1006

using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Anemonis.Extensions.RequestCompression.PerfTests;

public class RequestCompressionHttpMessageHandlerCodingTests
{
    private static readonly HttpResponseMessage _httpResponse = new();
    private static readonly DelegatingHandlerAdapter _httpHandlerAdapter = CreateHttpHandlerAdapter();

    private static DelegatingHandlerAdapter CreateHttpHandlerAdapter()
    {
        var compressionProviderRegistry = new TestRequestCompressionProviderRegistry();
        var logger = NullLogger.Instance;
        var httpMessageHandlerOptions = new RequestCompressionHttpMessageHandlerOptions();

        httpMessageHandlerOptions.EncodingName = "e";
        httpMessageHandlerOptions.CompressionLevel = CompressionLevel.NoCompression;
        httpMessageHandlerOptions.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, logger, httpMessageHandlerOptions);

        return new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHttpHandler);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task<HttpResponseMessage> PrimaryHttpHandler(HttpRequestMessage request)
    {
        await request.Content!.ReadAsStreamAsync();

        return _httpResponse;
    }

    [Benchmark(Description = "enc:n", Baseline = true)]
    public Task BenchmarkC0()
    {
        var request = new HttpRequestMessage();

        request.Content = new StringContent("", Encoding.UTF8, "application/xml");

        return _httpHandlerAdapter.SendAsync(request, default);
    }

    [Benchmark(Description = "enc:y")]
    public Task BenchmarkC1()
    {
        var request = new HttpRequestMessage();

        request.Content = new StringContent("", Encoding.UTF8, "application/json");

        return _httpHandlerAdapter.SendAsync(request, default);
    }

    private sealed class TestRequestCompressionProviderRegistry : IRequestCompressionProviderRegistry
    {
        private readonly IRequestCompressionProvider _provider = new TestCompressionProvider();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool TryGetProvider(string encodingName, out IRequestCompressionProvider? provider)
        {
            provider = _provider;

            return true;
        }
    }

    private sealed class TestCompressionProvider : IRequestCompressionProvider
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
        {
            return new DelegatingStream(outputStream);
        }

        public string EncodingName
        {
            get
            {
                return "e";
            }
        }
    }
}
