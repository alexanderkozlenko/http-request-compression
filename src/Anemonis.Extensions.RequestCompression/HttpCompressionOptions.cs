// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Specifies options to control the behavior of HTTP request compression. This class cannot be inherited.</summary>
public sealed class HttpCompressionOptions
{
    /// <summary>Initializes a new instance of the <see cref="HttpCompressionOptions" /> class.</summary>
    public HttpCompressionOptions()
    {
        CompressionProviders = new Dictionary<string, HttpCompressionProvider>(HttpCompressionDefaults.DefaultCompressionProviders, HttpCompressionDefaults.DefaultCompressionProviders.Comparer);
        MediaTypes = new HashSet<string>(HttpCompressionDefaults.DefaultMediaTypes, HttpCompressionDefaults.DefaultMediaTypes.Comparer);
        CompressionEncoding = HttpCompressionDefaults.DefaultCompressionEncoding;
        CompressionLevel = HttpCompressionDefaults.DefaultCompressionLevel;
    }

    /// <summary>Gets the collection of compression providers.</summary>
    /// <value>A dictionary with an encoding names as a key. The default is a dictionary with keys: "br", "deflate", "gzip".</value>
    public IDictionary<string, HttpCompressionProvider> CompressionProviders
    {
        get;
    }

    /// <summary>Gets the collection of media types eligible for compression.</summary>
    /// <value>A set of strings. The default is a set with values: "application/json".</value>
    public ISet<string> MediaTypes
    {
        get;
    }

    /// <summary>Gets or sets the compression encoding to use.</summary>
    /// <value>A single content type header value. The default is "br".</value>
    public string? CompressionEncoding
    {
        get;
        set;
    }

    /// <summary>Gets or sets the level of compression to use.</summary>
    /// <value>A compression level enumeration value. The default is <see cref="CompressionLevel.Fastest" />.</value>
    public CompressionLevel CompressionLevel
    {
        get;
        set;
    }
}
