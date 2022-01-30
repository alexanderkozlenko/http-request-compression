// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591
#pragma warning disable IDE0130

using System.Net.Mime;
using Anemonis.Extensions.RequestCompression;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class RequestCompressionServiceCollectionExtensions
{
    /// <summary>Adds HTTP request compression services to the collection of service descriptors.</summary>
    /// <returns>The current <see cref="IServiceCollection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services" /> is <see langword="null" />.</exception>
    public static IServiceCollection AddRequestCompression(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<RequestCompressionProviderRegistry>();
        services.Configure<RequestCompressionOptions>(Configure);

        return services;
    }

    /// <summary>Adds HTTP request compression services to the collection of service descriptors and configure options.</summary>
    /// <param name="configureOptions">The delegate to configure a <see cref="RequestCompressionOptions" /> instance.</param>
    /// <returns>The current <see cref="IServiceCollection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services" /> or <paramref name="configureOptions" /> is <see langword="null" />.</exception>
    public static IServiceCollection AddRequestCompression(this IServiceCollection services, Action<RequestCompressionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.TryAddSingleton<RequestCompressionProviderRegistry>();
        services.Configure<RequestCompressionOptions>(Configure);
        services.Configure(configureOptions);

        return services;
    }

    private static void Configure(RequestCompressionOptions options)
    {
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.DefaultMimeTypes.Add(MediaTypeNames.Application.Xml);
        options.DefaultMimeTypes.Add(MediaTypeNames.Application.Json);
    }
}
