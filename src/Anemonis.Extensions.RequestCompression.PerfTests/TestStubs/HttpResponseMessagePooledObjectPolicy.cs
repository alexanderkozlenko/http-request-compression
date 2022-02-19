using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class HttpResponseMessagePooledObjectPolicy : PooledObjectPolicy<HttpResponseMessage>
{
    public sealed override HttpResponseMessage Create()
    {
        return new();
    }

    public sealed override bool Return(HttpResponseMessage obj)
    {
        obj.Headers.Clear();
        obj.Content = null;

        return true;
    }
}
