#pragma warning disable CA1822

using System.IO.Compression;
using System.Runtime.CompilerServices;
using Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Anemonis.Extensions.RequestCompression.PerfTests;

public class RequestCompressionHttpMessageHandlerDiscoveryTests
{
    private static readonly DelegatingHandlerAdapter _httpHandlerAdapter0 = CreateHttpHandlerAdapter(PrimaryHttpHandler0);
    private static readonly DelegatingHandlerAdapter _httpHandlerAdapter1 = CreateHttpHandlerAdapter(PrimaryHttpHandler1);
    private static readonly DelegatingHandlerAdapter _httpHandlerAdapter2 = CreateHttpHandlerAdapter(PrimaryHttpHandler2);

    private static DelegatingHandlerAdapter CreateHttpHandlerAdapter(Func<HttpRequestMessage, Task<HttpResponseMessage>> primaryHandler)
    {
        var compressionProviderRegistry = new TestRequestCompressionProviderRegistry();
        var logger = NullLogger.Instance;
        var httpMessageHandlerOptions = new RequestCompressionHttpMessageHandlerOptions();

        httpMessageHandlerOptions.EncodingName = "e";
        httpMessageHandlerOptions.CompressionLevel = CompressionLevel.NoCompression;
        httpMessageHandlerOptions.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, httpMessageHandlerOptions, logger);

        return new DelegatingHandlerAdapter(httpMessageHandler, primaryHandler);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task<HttpResponseMessage> PrimaryHttpHandler0(HttpRequestMessage request)
    {
        var response = new HttpResponseMessage();

        return Task.FromResult(response);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task<HttpResponseMessage> PrimaryHttpHandler1(HttpRequestMessage request)
    {
        var response = new HttpResponseMessage();

        response.Headers.TryAddWithoutValidation("Accept-Encoding", "a");

        return Task.FromResult(response);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task<HttpResponseMessage> PrimaryHttpHandler2(HttpRequestMessage request)
    {
        var response = new HttpResponseMessage();

        response.Headers.TryAddWithoutValidation("Accept-Encoding", "a,b");

        return Task.FromResult(response);
    }

    [Benchmark(Description = "dis:n:-", Baseline = true)]
    public Task BenchmarkD0R0()
    {
        var request = new HttpRequestMessage();

        request.Method = HttpMethod.Options;

        return _httpHandlerAdapter0.SendAsync(request, default);
    }

    [Benchmark(Description = "dis:y:-")]
    public Task BenchmarkD1R0()
    {
        var request = new HttpRequestMessage();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, new());

        return _httpHandlerAdapter0.SendAsync(request, default);
    }

    [Benchmark(Description = "dis:y:1")]
    public Task BenchmarkD1R1()
    {
        var request = new HttpRequestMessage();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, new());

        return _httpHandlerAdapter1.SendAsync(request, default);
    }

    [Benchmark(Description = "dis:y:2")]
    public Task BenchmarkD1R2()
    {
        var request = new HttpRequestMessage();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, new());

        return _httpHandlerAdapter2.SendAsync(request, default);
    }

    private sealed class TestRequestCompressionProviderRegistry : IRequestCompressionProviderRegistry
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool TryGetProvider(string encodingName, out IRequestCompressionProvider? provider)
        {
            provider = null;

            return false;
        }
    }
}
