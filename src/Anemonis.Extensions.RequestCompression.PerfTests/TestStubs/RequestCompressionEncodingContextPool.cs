using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal static class RequestCompressionEncodingContextPool
{
    private static readonly ObjectPool<RequestCompressionEncodingContext> _shared = Create();

    private static ObjectPool<RequestCompressionEncodingContext> Create()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new RequestCompressionEncodingContextPooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<RequestCompressionEncodingContext> Shared
    {
        get
        {
            return _shared;
        }
    }
}
