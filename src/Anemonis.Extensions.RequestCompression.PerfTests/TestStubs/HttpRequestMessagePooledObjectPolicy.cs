using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class HttpRequestMessagePooledObjectPolicy : PooledObjectPolicy<HttpRequestMessage>
{
    public sealed override HttpRequestMessage Create()
    {
        return new();
    }

    public sealed override bool Return(HttpRequestMessage obj)
    {
        obj.Method = HttpMethod.Get;
        obj.Headers.Clear();
        obj.Content = null;

        ((IDictionary<string, object?>)obj.Options).Clear();

        return true;
    }
}
