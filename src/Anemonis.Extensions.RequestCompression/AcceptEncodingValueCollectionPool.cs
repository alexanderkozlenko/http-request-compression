// © Oleksandr Kozlenko. Licensed under the MIT license.

using System.Net.Http.Headers;
using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal static class AcceptEncodingValueCollectionPool
{
    private static readonly ObjectPool<HttpHeaderValueCollection<StringWithQualityHeaderValue>> _shared = CreateShared();

    private static ObjectPool<HttpHeaderValueCollection<StringWithQualityHeaderValue>> CreateShared()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new AcceptEncodingValueCollectionPooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<HttpHeaderValueCollection<StringWithQualityHeaderValue>> Shared
    {
        get
        {
            return _shared;
        }
    }
}
