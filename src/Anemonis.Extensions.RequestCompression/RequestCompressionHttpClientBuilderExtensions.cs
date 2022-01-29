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
    /// <returns>The current <see cref="IHttpClientBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> or <paramref name="encodingName" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddRequestCompressionHandler(this IHttpClientBuilder builder, string encodingName = "br", CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(encodingName);

        DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var compressionProviderRegistry = services.GetRequiredService<RequestCompressionProviderRegistry>();
            var compressionProvider = compressionProviderRegistry.GetProvider(encodingName);

            return new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel);
        }

        builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        return builder;
    }
}
