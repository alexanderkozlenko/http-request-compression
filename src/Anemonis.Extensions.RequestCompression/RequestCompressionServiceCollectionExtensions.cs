// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591
#pragma warning disable IDE0130

using System.IO.Compression;
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

        services.TryAddSingleton<IRequestCompressionProviderRegistry, RequestCompressionProviderRegistry>();
        services.TryAddSingleton<IRequestCompressionHttpMessageHandlerFactory, RequestCompressionHttpMessageHandlerFactory>();
        services.Configure<RequestCompressionOptions>(Configure);
        services.Configure<RequestCompressionOptions>(ConfigureHandler);
        services.PostConfigure<RequestCompressionOptions>(PostConfigure);
        services.PostConfigure<RequestCompressionOptions>(PostConfigureHandler);
        services.ConfigureAll<RequestCompressionHttpMessageHandlerOptions>(ConfigureHandler);

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

        services.TryAddSingleton<IRequestCompressionProviderRegistry, RequestCompressionProviderRegistry>();
        services.TryAddSingleton<IRequestCompressionHttpMessageHandlerFactory, RequestCompressionHttpMessageHandlerFactory>();
        services.Configure<RequestCompressionOptions>(Configure);
        services.Configure<RequestCompressionOptions>(ConfigureHandler);
        services.PostConfigure<RequestCompressionOptions>(PostConfigure);
        services.PostConfigure<RequestCompressionOptions>(PostConfigureHandler);
        services.ConfigureAll<RequestCompressionHttpMessageHandlerOptions>(ConfigureHandler);
        services.ConfigureAll(configureOptions);

        return services;
    }

    private static void Configure(RequestCompressionOptions options)
    {
        options.Providers.Add<BrotliRequestCompressionProvider>();
        options.Providers.Add<GzipRequestCompressionProvider>();
    }

    private static void ConfigureHandler(RequestCompressionHttpMessageHandlerOptions options)
    {
        options.EncodingName = "br";
        options.CompressionLevel = CompressionLevel.Fastest;
        options.MediaTypes.Add(MediaTypeNames.Application.Xml);
        options.MediaTypes.Add(MediaTypeNames.Application.Json);
    }

    private static void PostConfigure(RequestCompressionOptions options)
    {
        options.Providers.TrimExcess();
    }

    private static void PostConfigureHandler(RequestCompressionHttpMessageHandlerOptions options)
    {
        options.MediaTypes.TrimExcess();
    }
}
