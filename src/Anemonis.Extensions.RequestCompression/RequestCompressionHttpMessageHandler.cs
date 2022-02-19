// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandler : DelegatingHandler
{
    private readonly IRequestCompressionProviderRegistry _compressionProviderRegistry;
    private readonly RequestCompressionHttpMessageHandlerOptions _compressionOptions;
    private readonly ILogger _logger;

    public RequestCompressionHttpMessageHandler(IRequestCompressionProviderRegistry compressionProviderRegistry, RequestCompressionHttpMessageHandlerOptions compressionOptions, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(compressionProviderRegistry);
        ArgumentNullException.ThrowIfNull(compressionOptions);
        ArgumentNullException.ThrowIfNull(logger);

        _compressionProviderRegistry = compressionProviderRegistry;
        _compressionOptions = compressionOptions;
        _logger = logger;
    }

    private void ApplySelectedContentCoding(HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestContent = request.Content;

        if (requestContent is null)
        {
            return;
        }
        if (!request.Options.TryGetValue(RequestCompressionOptionKeys.CompressionEnabled, out var compressionEnabled))
        {
            compressionEnabled = ContentHasSupportedType(requestContent, _compressionOptions.MediaTypes);
        }
        if (!compressionEnabled)
        {
            return;
        }
        if (!request.Options.TryGetValue(RequestCompressionOptionKeys.EncodingName, out var encodingName))
        {
            encodingName = _compressionOptions.EncodingName;
        }
        if (encodingName is null)
        {
            return;
        }
        if (!_compressionProviderRegistry.TryGetProvider(encodingName, out var compressionProvider))
        {
            throw new InvalidOperationException($"No matching request compression provider found for encoding '{encodingName}'.");
        }
        if (compressionProvider is null)
        {
            return;
        }
        if (!request.Options.TryGetValue(RequestCompressionOptionKeys.CompressionLevel, out var compressionLevel))
        {
            compressionLevel = _compressionOptions.CompressionLevel;
        }

        request.Content = CreateCodingContent(requestContent, compressionProvider, compressionLevel);

        _logger.CompressingWith(compressionProvider.EncodingName);
    }

    private static bool ContentHasSupportedType(HttpContent content, ICollection<string> mediaTypes)
    {
        if (mediaTypes.Count == 0)
        {
            return false;
        }

        var mediaType = content.Headers.ContentType?.MediaType;

        if (mediaType is null)
        {
            return false;
        }

        return mediaTypes.Contains(mediaType);
    }

    private static HttpContent CreateCodingContent(HttpContent content, IRequestCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        var encodedContent = new CodingStreamContent(content, compressionProvider, compressionLevel);

        foreach (var (headerName, headerValues) in content.Headers.NonValidated)
        {
            encodedContent.Headers.TryAddWithoutValidation(headerName, headerValues);
        }

        encodedContent.Headers.ContentEncoding.Add(compressionProvider.EncodingName);
        encodedContent.Headers.ContentLength = null;

        return encodedContent;
    }

    private void FindSupportedContentCoding(HttpRequestMessage request, HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(response);

        if (!request.Options.TryGetValue(RequestCompressionOptionKeys.EncodingContext, out var encodingContext))
        {
            return;
        }
        if (encodingContext is null)
        {
            return;
        }

        // RFC 7231 "Hypertext Transfer Protocol (HTTP/1.1): Semantics and Content":
        //
        //     A server generating a successful response to OPTIONS SHOULD send any
        //     header fields that might indicate optional features implemented by
        //     the server and applicable to the target resource (e.g., Allow),
        //     including potential extensions not defined by this specification.
        //
        // RFC 7694 "Hypertext Transfer Protocol (HTTP) Client-Initiated Content-Encoding":
        //
        //     ... the header field can also be used to indicate to clients that content
        //     codings are supported, to optimize future interactions. For example,
        //     a resource might include it in a 2xx response ...

        if (!response.IsSuccessStatusCode || (request.Method != HttpMethod.Options))
        {
            return;
        }

        if (!response.Headers.NonValidated.TryGetValues("Accept-Encoding", out var responseHeaderValues))
        {
            return;
        }
        if (responseHeaderValues.Count == 0)
        {
            return;
        }

        var headerValues = AcceptEncodingValueCollectionPool.Shared.Get();

        foreach (var headerValue in responseHeaderValues)
        {
            headerValues.TryParseAdd(headerValue);
        }

        if (headerValues.Count != 0)
        {
            encodingContext.EncodingName = SelectSupportedContentCoding(headerValues);
        }

        AcceptEncodingValueCollectionPool.Shared.Return(headerValues);
    }

    private string? SelectSupportedContentCoding(ICollection<StringWithQualityHeaderValue> headerValues)
    {
        if (headerValues.Count == 1)
        {
            var headerValue = headerValues.First();
            var encodingName = headerValue.Value;

            if (_compressionProviderRegistry.TryGetProvider(encodingName, out var compressionProvider))
            {
                return compressionProvider?.EncodingName;
            }
            if (string.Equals(encodingName, ContentCodingTokens.Asterisk, StringComparison.Ordinal))
            {
                return _compressionOptions.EncodingName;
            }
        }
        else if (headerValues.Count > 1)
        {
            var priorityQueue = ContentCodingPriorityQueuePool.Shared.Get();
            var priorityQueueIsRetainable = headerValues.Count <= ContentCodingPriorityQueuePooledObjectPolicy.MaximumRetainedCapacity;

            priorityQueue.EnsureCapacity(headerValues.Count);

            foreach (var headerValue in headerValues)
            {
                priorityQueue.Enqueue(headerValue.Value, -(headerValue.Quality ?? 1D));
            }

            while (priorityQueue.Count != 0)
            {
                var encodingName = priorityQueue.Peek();

                if (_compressionProviderRegistry.TryGetProvider(encodingName, out var compressionProvider))
                {
                    if (priorityQueueIsRetainable)
                    {
                        ContentCodingPriorityQueuePool.Shared.Return(priorityQueue);
                    }

                    return compressionProvider?.EncodingName;
                }
                if (string.Equals(encodingName, ContentCodingTokens.Asterisk, StringComparison.Ordinal))
                {
                    if (priorityQueueIsRetainable)
                    {
                        ContentCodingPriorityQueuePool.Shared.Return(priorityQueue);
                    }

                    return _compressionOptions.EncodingName;
                }

                priorityQueue.Dequeue();
            }

            if (priorityQueueIsRetainable)
            {
                ContentCodingPriorityQueuePool.Shared.Return(priorityQueue);
            }
        }

        return null;
    }

    protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplySelectedContentCoding(request);

        var response = base.Send(request, cancellationToken);

        FindSupportedContentCoding(request, response);

        return response;
    }

    protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplySelectedContentCoding(request);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        FindSupportedContentCoding(request, response);

        return response;
    }
}
