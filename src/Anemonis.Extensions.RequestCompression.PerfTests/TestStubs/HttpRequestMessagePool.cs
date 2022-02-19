using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal static class HttpRequestMessagePool
{
    private static readonly ObjectPool<HttpRequestMessage> _shared = Create();

    private static ObjectPool<HttpRequestMessage> Create()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new HttpRequestMessagePooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<HttpRequestMessage> Shared
    {
        get
        {
            return _shared;
        }
    }
}
