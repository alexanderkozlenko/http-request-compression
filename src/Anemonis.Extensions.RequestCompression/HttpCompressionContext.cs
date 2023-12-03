// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Represents a context for HTTP request compression operations. This class cannot be inherited.</summary>
public sealed class HttpCompressionContext
{
    /// <summary>Initializes a new instance of the <see cref="HttpCompressionContext" /> class.</summary>
    public HttpCompressionContext()
    {
    }

    /// <summary>Gets or sets the compression encoding to use.</summary>
    /// <value>A single content type header value or <see langword="null" />.</value>
    public string? CompressionEncoding
    {
        get;
        set;
    }
}
