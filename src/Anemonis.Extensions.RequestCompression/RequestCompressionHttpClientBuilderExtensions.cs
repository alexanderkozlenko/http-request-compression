// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591
#pragma warning disable IDE0130

using Anemonis.Extensions.RequestCompression;

namespace Microsoft.Extensions.DependencyInjection;

public static class RequestCompressionHttpClientBuilderExtensions
{
    /// <summary>Adds a message handler to provide HTTP request compression for a named <see cref="HttpClient" />.</summary>
    /// <returns>The current <see cref="IHttpClientBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddRequestCompressionHandler(this IHttpClientBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        static DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var factory = services.GetRequiredService<IRequestCompressionHttpMessageHandlerFactory>();

            return factory.CreateHandler(Options.Options.DefaultName);
        }

        builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        return builder;
    }

    /// <summary>Adds a message handler to provide HTTP request compression for a named <see cref="HttpClient" />.</summary>
    /// <param name="configureOptions">The delegate to configure a <see cref="RequestCompressionHttpMessageHandlerOptions" /> instance.</param>
    /// <returns>The current <see cref="IHttpClientBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> or <paramref name="configureOptions" /> is <see langword="null" />.</exception>
    public static IHttpClientBuilder AddRequestCompressionHandler(this IHttpClientBuilder builder, Action<RequestCompressionHttpMessageHandlerOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureOptions);

        var instanceName = builder.Name;

        builder.Services.PostConfigure(instanceName, configureOptions);
        builder.Services.PostConfigure<RequestCompressionHttpMessageHandlerOptions>(instanceName, PostConfigure);

        DelegatingHandler CreateHttpMessageHandler(IServiceProvider services)
        {
            var handlerFactory = services.GetRequiredService<IRequestCompressionHttpMessageHandlerFactory>();

            return handlerFactory.CreateHandler(instanceName);
        }

        builder.AddHttpMessageHandler(CreateHttpMessageHandler);

        return builder;
    }

    private static void PostConfigure(RequestCompressionHttpMessageHandlerOptions options)
    {
        options.MediaTypes.TrimExcess();
    }
}
