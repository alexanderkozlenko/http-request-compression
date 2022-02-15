// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal static class ContentCodingPriorityQueuePool
{
    private static readonly ObjectPool<PriorityQueue<string, double>> _shared = CreateShared();

    private static ObjectPool<PriorityQueue<string, double>> CreateShared()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new ContentCodingPriorityQueuePooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<PriorityQueue<string, double>> Shared
    {
        get
        {
            return _shared;
        }
    }
}
