// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Provides extension methods for an HTTP client builder that add an HTTP message handler with the request compression feature.</summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>Adds a delegate that will be used to create an HTTP message handler with the request compression feature.</summary>
    /// <param name="builder">An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</param>
    /// <param name="mediaTypes">The collection of media types eligible for compression.</param>
    /// <param name="compressionEncoding">The compression encoding to apply.</param>
    /// <param name="compressionLevel">The level of compression to use.</param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddCompressionHandler(this IHttpClientBuilder builder, IEnumerable<string>? mediaTypes = null, string? compressionEncoding = null, CompressionLevel? compressionLevel = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var handlerCompressionProviders = HttpCompressionOptions.DefaultCompressionProviders;
            var handlerMediaTypes = HttpCompressionOptions.DefaultMediaTypes;
            var handlerCompressionEncoding = HttpCompressionOptions.DefaultCompressionEncoding;
            var handlerCompressionLevel = HttpCompressionOptions.DefaultCompressionLevel;

            var compressionOptions = services.GetService<IOptions<HttpCompressionOptions>>();

            if (compressionOptions is not null)
            {
                handlerCompressionProviders = (Dictionary<string, HttpCompressionProvider>)compressionOptions.Value.CompressionProviders;
                handlerMediaTypes = (HashSet<string>)compressionOptions.Value.MediaTypes;
                handlerCompressionEncoding = compressionOptions.Value.CompressionEncoding ?? HttpCompressionOptions.DefaultCompressionEncoding;
                handlerCompressionLevel = compressionOptions.Value.CompressionLevel;
            }

            if (mediaTypes is not null)
            {
                handlerMediaTypes = new(mediaTypes, StringComparer.OrdinalIgnoreCase);
            }

            if (!string.IsNullOrEmpty(compressionEncoding))
            {
                handlerCompressionEncoding = compressionEncoding;
            }

            if (compressionLevel.HasValue)
            {
                handlerCompressionLevel = compressionLevel.Value;
            }

            return new HttpCompressionHandler(handlerCompressionProviders, handlerMediaTypes, handlerCompressionEncoding, handlerCompressionLevel);
        }
    }
}
