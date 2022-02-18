// © Oleksandr Kozlenko. Licensed under the MIT license.

using System.Net.Http.Headers;
using Microsoft.Extensions.ObjectPool;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class AcceptEncodingValueCollectionPooledObjectPolicy : PooledObjectPolicy<HttpHeaderValueCollection<StringWithQualityHeaderValue>>
{
    public AcceptEncodingValueCollectionPooledObjectPolicy()
    {
    }

    public sealed override HttpHeaderValueCollection<StringWithQualityHeaderValue> Create()
    {
        using var httpRequestMessage = new HttpRequestMessage();

        return httpRequestMessage.Headers.AcceptEncoding;
    }

    public sealed override bool Return(HttpHeaderValueCollection<StringWithQualityHeaderValue> obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        obj.Clear();

        return true;
    }
}
