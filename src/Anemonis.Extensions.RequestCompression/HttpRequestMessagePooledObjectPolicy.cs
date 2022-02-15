// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class HttpRequestMessagePooledObjectPolicy : PooledObjectPolicy<HttpRequestMessage>
{
    public sealed override HttpRequestMessage Create()
    {
        return new();
    }

    public sealed override bool Return(HttpRequestMessage obj)
    {
        obj.Headers.Clear();

        return true;
    }
}
