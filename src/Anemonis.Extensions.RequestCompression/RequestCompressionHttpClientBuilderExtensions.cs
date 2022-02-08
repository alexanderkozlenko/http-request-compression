// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591
#pragma warning disable IDE0130

using System.IO.Compression;
using Anemonis.Extensions.RequestCompression;

namespace Microsoft.Extensions.DependencyInjection;

public static class RequestCompressionHttpClientBuilderExtensions
{
    /// <summary>Adds a message handler to provide request compression for a named <see cref="HttpClient" />.</summary>
    /// <param name="encodingName">The encoding token that defines a compression format.</param>
    /// <param name="compressionLevel">The level of compression for the defined format, if applicable.</param>
    /// <param name="mediaTypes">The collection of Content-Type media types to compress.</param>
    /// <returns>The current <see cref="IHttpClientBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddRequestCompressionHandler(this IHttpClientBuilder builder, string? encodingName = null, CompressionLevel? compressionLevel = null, IEnumerable<string>? mediaTypes = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (mediaTypes is not null and not RequestCompressionMediaTypeCollection)
        {
            mediaTypes = new RequestCompressionMediaTypeCollection(mediaTypes);
        }

        DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var httpMessageHandlerFactory = services.GetRequiredService<IRequestCompressionHttpMessageHandlerFactory>();

            return httpMessageHandlerFactory.CreateHandler(encodingName, compressionLevel, mediaTypes as RequestCompressionMediaTypeCollection);
        }

        builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        return builder;
    }
}
