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

    private static DelegatingHandlerAdapter CreateHttpHandlerAdapter(Func<HttpRequestMessage, HttpResponseMessage> primaryHandler)
    {
        var compressionProviderRegistry = new TestRequestCompressionProviderRegistry();
        var httpMessageHandlerOptions = new RequestCompressionHttpMessageHandlerOptions();
        var logger = NullLogger.Instance;

        httpMessageHandlerOptions.EncodingName = "e";
        httpMessageHandlerOptions.CompressionLevel = CompressionLevel.NoCompression;
        httpMessageHandlerOptions.MediaTypes.Add("text/plain");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, httpMessageHandlerOptions, logger);

        return new DelegatingHandlerAdapter(httpMessageHandler, primaryHandler);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static HttpResponseMessage PrimaryHttpHandler0(HttpRequestMessage request)
    {
        var response = HttpResponseMessagePool.Shared.Get();

        return response;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static HttpResponseMessage PrimaryHttpHandler1(HttpRequestMessage request)
    {
        var response = HttpResponseMessagePool.Shared.Get();

        response.Headers.TryAddWithoutValidation("Accept-Encoding", "a");

        return response;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static HttpResponseMessage PrimaryHttpHandler2(HttpRequestMessage request)
    {
        var response = HttpResponseMessagePool.Shared.Get();

        response.Headers.TryAddWithoutValidation("Accept-Encoding", "a,b");

        return response;
    }

    [Benchmark(Description = "dis:n:-", Baseline = true)]
    public void BenchmarkD0R0()
    {
        var request = HttpRequestMessagePool.Shared.Get();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, null);

        var response = _httpHandlerAdapter0.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
    }

    [Benchmark(Description = "dis:y:-")]
    public void BenchmarkD1R0()
    {
        var context = RequestCompressionEncodingContextPool.Shared.Get();
        var request = HttpRequestMessagePool.Shared.Get();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, context);

        var response = _httpHandlerAdapter0.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
        RequestCompressionEncodingContextPool.Shared.Return(context);
    }

    [Benchmark(Description = "dis:y:1")]
    public void BenchmarkD1R1()
    {
        var context = RequestCompressionEncodingContextPool.Shared.Get();
        var request = HttpRequestMessagePool.Shared.Get();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, context);

        var response = _httpHandlerAdapter1.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
        RequestCompressionEncodingContextPool.Shared.Return(context);
    }

    [Benchmark(Description = "dis:y:2")]
    public void BenchmarkD1R2()
    {
        var context = RequestCompressionEncodingContextPool.Shared.Get();
        var request = HttpRequestMessagePool.Shared.Get();

        request.Method = HttpMethod.Options;
        request.Options.Set(RequestCompressionOptionKeys.EncodingContext, context);

        var response = _httpHandlerAdapter2.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
        RequestCompressionEncodingContextPool.Shared.Return(context);
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
