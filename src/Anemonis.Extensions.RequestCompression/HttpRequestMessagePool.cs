// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal static class HttpRequestMessagePool
{
    private static readonly ObjectPool<HttpRequestMessage> _shared = CreateShared();

    private static ObjectPool<HttpRequestMessage> CreateShared()
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
