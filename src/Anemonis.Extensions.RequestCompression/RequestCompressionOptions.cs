// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public class RequestCompressionOptions : RequestCompressionHttpMessageHandlerOptions
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
