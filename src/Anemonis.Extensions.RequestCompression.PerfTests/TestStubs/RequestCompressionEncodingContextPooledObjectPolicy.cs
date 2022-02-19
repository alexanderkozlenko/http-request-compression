using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class RequestCompressionEncodingContextPooledObjectPolicy : PooledObjectPolicy<RequestCompressionEncodingContext>
{
    public sealed override RequestCompressionEncodingContext Create()
    {
        return new();
    }

    public sealed override bool Return(RequestCompressionEncodingContext obj)
    {
        obj.EncodingName = null;

        return true;
    }
}
