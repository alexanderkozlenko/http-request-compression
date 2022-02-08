// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandler : DelegatingHandler
{
    public static readonly HttpRequestOptionsKey<bool> EnableCompressionOptionKey = new($"{typeof(RequestCompressionHttpMessageHandler).Namespace}.EnableCompression");

    private readonly IRequestCompressionProvider _compressionProvider;
    private readonly CompressionLevel _compressionLevel;
    private readonly ICollection<string> _mediaTypes;
    private readonly ILogger _logger;

    public RequestCompressionHttpMessageHandler(IRequestCompressionProvider compressionProvider, CompressionLevel compressionLevel, ICollection<string> mediaTypes, ILogger logger)
    {
        _compressionProvider = compressionProvider;
        _compressionLevel = compressionLevel;
        _mediaTypes = mediaTypes;
        _logger = logger;
    }

    protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request?.Content is { } content)
        {
            var enableCompression = GetEnableCompressionOption(request.Options);

            if (((enableCompression is null) && HasAllowedContentType(content)) || (enableCompression is true))
            {
                request.Content = CreateCompressionStreamContent(content);

                _logger.AddingCompression(_compressionProvider.EncodingName);
            }
        }

        return base.Send(request!, cancellationToken);
    }

    protected sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request?.Content is { } content)
        {
            var enableCompression = GetEnableCompressionOption(request.Options);

            if (((enableCompression is null) && HasAllowedContentType(content)) || (enableCompression is true))
            {
                request.Content = CreateCompressionStreamContent(content);

                _logger.AddingCompression(_compressionProvider.EncodingName);
            }
        }

        return base.SendAsync(request!, cancellationToken);
    }

    private HttpContent CreateCompressionStreamContent(HttpContent originalContent)
    {
        var compressionContent = new CompressionStreamContent(originalContent, _compressionProvider, _compressionLevel);

        foreach (var (headerName, headerValues) in originalContent.Headers.NonValidated)
        {
            compressionContent.Headers.TryAddWithoutValidation(headerName, headerValues);
        }

        compressionContent.Headers.ContentEncoding.Add(_compressionProvider.EncodingName);
        compressionContent.Headers.ContentLength = null;

        return compressionContent;
    }

    private bool HasAllowedContentType(HttpContent content)
    {
        if (_mediaTypes.Count == 0)
        {
            return false;
        }

        var mediaType = content.Headers.ContentType?.MediaType;

        if (mediaType is null)
        {
            return false;
        }

        return _mediaTypes.Contains(mediaType);
    }

    private static bool? GetEnableCompressionOption(HttpRequestOptions options)
    {
        if (options.TryGetValue(EnableCompressionOptionKey, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }
}
