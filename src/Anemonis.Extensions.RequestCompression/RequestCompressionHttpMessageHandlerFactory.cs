// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandlerFactory : IRequestCompressionHttpMessageHandlerFactory
{
    private readonly IOptions<RequestCompressionOptions> _options;
    private readonly IRequestCompressionProviderRegistry _compressionProviderRegistry;
    private readonly ILogger _logger;

    public RequestCompressionHttpMessageHandlerFactory(IOptions<RequestCompressionOptions> options, IRequestCompressionProviderRegistry compressionProviderRegistry, ILoggerFactory loggerFactory)
    {
        _options = options;
        _compressionProviderRegistry = compressionProviderRegistry;
        _logger = loggerFactory.CreateLogger<RequestCompressionHttpMessageHandler>();
    }

    public DelegatingHandler CreateHandler(string? encodingName, CompressionLevel? compressionLevel, IEnumerable<string>? mediaTypes)
    {
        var options = _options.Value;

        encodingName ??= options.DefaultEncodingName;
        encodingName ??= "br";

        var compressionProvider = _compressionProviderRegistry.GetProvider(encodingName);

        compressionLevel ??= options.DefaultCompressionLevel;
        compressionLevel ??= CompressionLevel.Fastest;

        mediaTypes ??= options.DefaultMediaTypes;

        return new RequestCompressionHttpMessageHandler(compressionProvider, compressionLevel.Value, mediaTypes, _logger);
    }
}
