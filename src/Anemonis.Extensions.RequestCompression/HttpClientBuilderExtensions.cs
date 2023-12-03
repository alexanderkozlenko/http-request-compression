// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Frozen;
using Anemonis.Extensions.RequestCompression;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Provides extension methods for the <see cref="IHttpClientBuilder" /> interface that enable HTTP request compression. This class cannot be inherited.</summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>Adds a message handler for transparent HTTP request compression.</summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The value of <paramref name="builder" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddCompressionHandler(this IHttpClientBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHttpMessageHandler(static services => CreateCompressionHandler(services, Options.Options.DefaultName));
    }

    /// <summary>Adds a message handler for transparent HTTP request compression.</summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="configure">The callback that configures the handler.</param>
    /// <returns>The value of <paramref name="builder" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> or <paramref name="configure" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddCompressionHandler(this IHttpClientBuilder builder, Action<HttpCompressionOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var optionsName = GetOptionsName(builder.Name);

        builder.Services.AddOptions<HttpCompressionOptions>(optionsName).Configure(configure);

        return builder.AddHttpMessageHandler(services => CreateCompressionHandler(services, optionsName));
    }

    /// <summary>Adds a message handler for transparent HTTP request compression.</summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="configure">The callback that configures the handler.</param>
    /// <returns>The value of <paramref name="builder" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> or <paramref name="configure" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddCompressionHandler(this IHttpClientBuilder builder, Action<HttpCompressionOptions, IServiceProvider> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var optionsName = GetOptionsName(builder.Name);

        builder.Services.AddOptions<HttpCompressionOptions>(optionsName).Configure(configure);

        return builder.AddHttpMessageHandler(services => CreateCompressionHandler(services, optionsName));
    }

    private static HttpCompressionHandler CreateCompressionHandler(IServiceProvider services, string? optionsName)
    {
        var optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<HttpCompressionOptions>>();
        var options = optionsSnapshot.Get(optionsName);

        return new(
            options.CompressionProviders.ToFrozenDictionary(),
            options.MediaTypes.ToFrozenSet(),
            options.CompressionEncoding ?? "identity",
            options.CompressionLevel);
    }

    private static string GetOptionsName(string httpClientName)
    {
        return $"{httpClientName}-{Guid.NewGuid()}";
    }
}
