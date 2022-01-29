// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionOptions
{
    public RequestCompressionOptions()
    {
        Providers = new();
    }

    public RequestCompressionProviderCollection Providers
    {
        get;
    }
}
