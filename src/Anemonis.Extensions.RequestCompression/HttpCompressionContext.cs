// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Represents a context for HTTP request compression operations.</summary>
public sealed class HttpCompressionContext
{
    /// <summary>Initializes a new instance of the <see cref="HttpCompressionContext" /> class.</summary>
    public HttpCompressionContext()
    {
    }

    /// <summary>Gets or sets an available compression encoding that is supported by a server.</summary>
    /// <value>A single content type header value.</value>
    public string? CompressionEncoding
    {
        get;
        set;
    }
}
