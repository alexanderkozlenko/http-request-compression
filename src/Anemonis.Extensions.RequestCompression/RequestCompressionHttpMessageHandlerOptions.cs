// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public class RequestCompressionHttpMessageHandlerOptions
{
    public RequestCompressionHttpMessageHandlerOptions()
    {
        MediaTypes = new();
    }

    /// <summary>Gets or sets the encoding token that defines a compression format.</summary>
    public string? EncodingName
    {
        get;
        set;
    }

    /// <summary>Gets or sets the level of compression for the compression format.</summary>
    public CompressionLevel CompressionLevel
    {
        get;
        set;
    }

    /// <summary>Gets the collection of media types to compress.</summary>
    public RequestCompressionMediaTypeCollection MediaTypes
    {
        get;
    }
}
