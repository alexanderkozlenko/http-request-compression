// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandlerFactory : IRequestCompressionHttpMessageHandlerFactory
{
    private readonly IRequestCompressionProviderRegistry _compressionProviderRegistry;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions> _options;

    public RequestCompressionHttpMessageHandlerFactory(IRequestCompressionProviderRegistry compressionProviderRegistry, ILoggerFactory loggerFactory, IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(compressionProviderRegistry);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        _compressionProviderRegistry = compressionProviderRegistry;
        _loggerFactory = loggerFactory;
        _options = options;
    }

    public DelegatingHandler CreateHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var logger = _loggerFactory.CreateLogger<RequestCompressionHttpMessageHandler>();
        var options = _options.Get(name);

        return new RequestCompressionHttpMessageHandler(_compressionProviderRegistry, logger, options);
    }
}
