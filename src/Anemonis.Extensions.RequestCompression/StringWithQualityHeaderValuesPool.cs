// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal static class StringWithQualityHeaderValuesPool
{
    private static readonly ObjectPool<List<(string, double)>> _shared = Create();

    private static ObjectPool<List<(string, double)>> Create()
    {
        var objectPoolProvider = new DefaultObjectPoolProvider();
        var objectPoolPolicy = new StringWithQualityHeaderValuesPooledObjectPolicy();
        var objectPool = objectPoolProvider.Create(objectPoolPolicy);

        return objectPool;
    }

    public static ObjectPool<List<(string, double)>> Shared
    {
        get
        {
            return _shared;
        }
    }
}
