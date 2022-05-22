// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionEncodingContext
{
    public RequestCompressionEncodingContext()
    {
    }

    /// <summary>Gets or sets the encoding token that defines a preferrable compression format supported by client and server.</summary>
    public string? EncodingName
    {
        get;
        set;
    }

    /// <summary>Gets or sets the flag that indicates whether server supports discovery of compression formats supported.</summary>
    public bool IsDiscoverySupported
    {
        get;
        set;
    }
}
