// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class ContentCodingPriorityQueuePooledObjectPolicy : PooledObjectPolicy<PriorityQueue<string, double>>
{
    public const int MaximumRetainedCapacity = 16;

    public sealed override PriorityQueue<string, double> Create()
    {
        return new(MaximumRetainedCapacity);
    }

    public sealed override bool Return(PriorityQueue<string, double> obj)
    {
        obj.Clear();

        return true;
    }
}
