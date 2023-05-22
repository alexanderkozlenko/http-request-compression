// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public static class RequestCompressionHttpRequestOptionsExtensions
{
    /// <summary>Toggles compression for the HTTP request.</summary>
    /// <param name="compressionEnabled"><see langword="true" /> if compression is enabled; otherwise <see langword="false" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
    public static void SetCompressionEnabled(this HttpRequestOptions options, bool compressionEnabled)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Set(RequestCompressionOptionKeys.CompressionEnabled, compressionEnabled);
    }

    /// <summary>Specifies compression encoding for the HTTP request.</summary>
    /// <param name="encodingName">The encoding token that defines a compression format.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
    public static void SetCompressionEncoding(this HttpRequestOptions options, string? encodingName)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Set(RequestCompressionOptionKeys.EncodingName, encodingName);
    }

    /// <summary>Specifies compression level for the HTTP request.</summary>
    /// <param name="compressionLevel">The level of compression for the compression format.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
    public static void SetCompressionLevel(this HttpRequestOptions options, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Set(RequestCompressionOptionKeys.CompressionLevel, compressionLevel);
    }

    /// <summary>Adds a context to store information about HTTP request compression format supported by client and server.</summary>
    /// <param name="context">The context with information about supported compression format.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
    public static void AddCompressionDiscovery(this HttpRequestOptions options, out RequestCompressionEncodingContext context)
    {
        ArgumentNullException.ThrowIfNull(options);

        context = new();
        options.Set(RequestCompressionOptionKeys.EncodingContext, context);
    }
}
