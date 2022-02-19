// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandlerFactory : IRequestCompressionHttpMessageHandlerFactory
{
    private readonly IRequestCompressionProviderRegistry _compressionProviderRegistry;
    private readonly IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions> _optionsMonitor;
    private readonly ILoggerFactory _loggerFactory;

    public RequestCompressionHttpMessageHandlerFactory(IRequestCompressionProviderRegistry compressionProviderRegistry, IOptionsMonitor<RequestCompressionHttpMessageHandlerOptions> optionsMonitor, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(compressionProviderRegistry);
        ArgumentNullException.ThrowIfNull(optionsMonitor);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _compressionProviderRegistry = compressionProviderRegistry;
        _optionsMonitor = optionsMonitor;
        _loggerFactory = loggerFactory;
    }

    public DelegatingHandler CreateHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var compressionOptions = _optionsMonitor.Get(name);
        var logger = _loggerFactory.CreateLogger<RequestCompressionHttpMessageHandler>();

        return new RequestCompressionHttpMessageHandler(_compressionProviderRegistry, compressionOptions, logger);
    }
}
