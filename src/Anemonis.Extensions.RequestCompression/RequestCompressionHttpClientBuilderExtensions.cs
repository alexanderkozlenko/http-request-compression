// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591
#pragma warning disable IDE0130

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class RequestCompressionHttpClientBuilderExtensions
{
    /// <summary>Adds a message handler to provide request compression for a named <see cref="HttpClient" />.</summary>
    /// <param name="encodingName">The encoding token that defines a compression format.</param>
    /// <param name="compressionLevel">The level of compression for the defined format, if applicable.</param>
    /// <param name="mimeTypes">The collection of Content-Type MIME types to compress.</param>
    /// <returns>The current <see cref="IHttpClientBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddRequestCompressionHandler(this IHttpClientBuilder builder, string? encodingName = null, CompressionLevel? compressionLevel = null, IEnumerable<string>? mimeTypes = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (mimeTypes is not null)
        {
            mimeTypes = new HashSet<string>(mimeTypes, StringComparer.OrdinalIgnoreCase);
        }

        DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var compressionOptions = services.GetRequiredService<IOptions<RequestCompressionOptions>>().Value;

            encodingName ??= compressionOptions.DefaultEncodingName;
            encodingName ??= "br";

            var compressionProviderRegistry = services.GetRequiredService<RequestCompressionProviderRegistry>();
            var compressionProvider = compressionProviderRegistry.GetProvider(encodingName);

            compressionLevel ??= compressionOptions.DefaultCompressionLevel;
            compressionLevel ??= CompressionLevel.Fastest;
            mimeTypes ??= compressionOptions.DefaultMimeTypes;

            var logger = services.GetService<ILogger<RequestCompressionHttpMessageHandler>>();

            return new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel.Value, mimeTypes, logger);
        }

        builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        return builder;
    }
}
