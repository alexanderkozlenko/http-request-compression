#pragma warning disable CA1822

using System.IO.Compression;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Anemonis.Extensions.RequestCompression.PerfTests;

public class RequestCompressionHttpMessageHandlerCodingTests
{
    private static readonly MediaTypeHeaderValue _contentType0 = new("application/bson");
    private static readonly MediaTypeHeaderValue _contentType1 = new("application/json");
    private static readonly DelegatingHandlerAdapter _httpHandlerAdapter = CreateHttpHandlerAdapter();

    private static DelegatingHandlerAdapter CreateHttpHandlerAdapter()
    {
        var compressionProviderRegistry = new TestRequestCompressionProviderRegistry();
        var httpMessageHandlerOptions = new RequestCompressionHttpMessageHandlerOptions();
        var logger = NullLogger.Instance;

        httpMessageHandlerOptions.EncodingName = "e";
        httpMessageHandlerOptions.CompressionLevel = CompressionLevel.NoCompression;
        httpMessageHandlerOptions.MediaTypes.Add("application/json");

        var httpMessageHandler = new RequestCompressionHttpMessageHandler(compressionProviderRegistry, httpMessageHandlerOptions, logger);

        return new DelegatingHandlerAdapter(httpMessageHandler, PrimaryHttpHandler);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static HttpResponseMessage PrimaryHttpHandler(HttpRequestMessage request)
    {
        request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();

        return HttpResponseMessagePool.Shared.Get();
    }

    [Benchmark(Description = "enc:n", Baseline = true)]
    public void BenchmarkC0()
    {
        var request = HttpRequestMessagePool.Shared.Get();

        request.Content = new StreamContent(Stream.Null);
        request.Content.Headers.ContentType = _contentType0;

        var response = _httpHandlerAdapter.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
    }

    [Benchmark(Description = "enc:y")]
    public void BenchmarkC1()
    {
        var request = HttpRequestMessagePool.Shared.Get();

        request.Content = new StreamContent(Stream.Null);
        request.Content.Headers.ContentType = _contentType1;

        var response = _httpHandlerAdapter.InvokeSend(request, default);

        HttpResponseMessagePool.Shared.Return(response);
        HttpRequestMessagePool.Shared.Return(request);
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
