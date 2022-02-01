// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionOptions
{
    public RequestCompressionOptions()
    {
        Providers = new();
        DefaultMediaTypes = new();
    }

    public RequestCompressionProviderCollection Providers
    {
        get;
    }

    /// <summary>Gets or sets the default encoding token that defines a compression format.</summary>
    public string? DefaultEncodingName
    {
        get;
        set;
    }

    /// <summary>Gets or sets the default level of compression for the defined format, if applicable.</summary>
    public CompressionLevel? DefaultCompressionLevel
    {
        get;
        set;
    }

    /// <summary>Gets the collection of default Content-Type media types to compress.</summary>
    public RequestCompressionMediaTypeCollection DefaultMediaTypes
    {
        get;
    }
}
