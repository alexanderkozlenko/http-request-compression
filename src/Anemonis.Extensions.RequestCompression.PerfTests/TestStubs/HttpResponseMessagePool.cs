using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal static class HttpResponseMessagePool
{
    private static readonly ObjectPool<HttpResponseMessage> _shared = Create();

    private static ObjectPool<HttpResponseMessage> Create()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new HttpResponseMessagePooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<HttpResponseMessage> Shared
    {
        get
        {
            return _shared;
        }
    }
}
