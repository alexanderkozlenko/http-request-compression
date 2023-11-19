// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;
using System.Net.Mime;
using Anemonis.Extensions.RequestCompression.Providers;

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Specifies options to control the behavior of HTTP request content compression. This class cannot be inherited.</summary>
public sealed class HttpCompressionOptions
{
    internal const string DefaultCompressionEncoding = "br";
    internal const CompressionLevel DefaultCompressionLevel = CompressionLevel.Fastest;

    internal static readonly Dictionary<string, HttpCompressionProvider> DefaultCompressionProviders = new(StringComparer.OrdinalIgnoreCase)
    {
        ["br"] = new BrotliCompressionProvider(),
        ["deflate"] = new DeflateCompressionProvider(),
        ["gzip"] = new GzipCompressionProvider(),
    };

    internal static readonly HashSet<string> DefaultMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        MediaTypeNames.Application.Json,
    };

    /// <summary>Initializes a new instance of the <see cref="HttpCompressionOptions" /> class.</summary>
    public HttpCompressionOptions()
    {
        CompressionProviders = new Dictionary<string, HttpCompressionProvider>(DefaultCompressionProviders, StringComparer.OrdinalIgnoreCase);
        MediaTypes = new HashSet<string>(DefaultMediaTypes, StringComparer.OrdinalIgnoreCase);
        CompressionEncoding = DefaultCompressionEncoding;
        CompressionLevel = DefaultCompressionLevel;
    }

    /// <summary>Gets the collection of available compression providers.</summary>
    /// <value>A dictionary with an encoding names as a key. The dictionary has case-insensitive string comparison for keys. The default is a dictionary instance with the following keys: "br", "deflate", "gzip".</value>
    public IDictionary<string, HttpCompressionProvider> CompressionProviders
    {
        get;
    }

    /// <summary>Gets the collection of default media types eligible for compression.</summary>
    /// <value>A set of strings. The set has case-insensitive string comparison. The default is a set with the following values: "application/json".</value>
    public ISet<string> MediaTypes
    {
        get;
    }

    /// <summary>Gets or sets the default compression encoding to use.</summary>
    /// <value>A single content type header value. The default is "br".</value>
    public string? CompressionEncoding
    {
        get;
        set;
    }

    /// <summary>Gets or sets the default level of compression to use.</summary>
    /// <value>A compression level enumeration value. The default is <see cref="CompressionLevel.Fastest" />.</value>
    public CompressionLevel CompressionLevel
    {
        get;
        set;
    }
}
