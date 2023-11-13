// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandler : DelegatingHandler
{
    private static readonly char[] s_codingTokenSeparators = { ',' };

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

    private static double CreateCanonicalQualityValue(double? value)
    {
        if (value is null)
        {
            return QualityValues.MaxValue;
        }
        else
        {
            return Math.Round(value.Value, 3, MidpointRounding.AwayFromZero);
        }
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

    private void FindSupportedContentCoding(HttpRequestMessage request, HttpResponseMessage response, RequestCompressionEncodingContext? encodingContext)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(response);

        if (encodingContext is null)
        {
            return;
        }

        encodingContext.EncodingName = null;

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

        if (!response.Headers.NonValidated.TryGetValues("Accept-Encoding", out var headerValues))
        {
            return;
        }

        encodingContext.IsDiscoverySupported = true;
        encodingContext.EncodingName = SelectSupportedContentCoding(headerValues);
    }

    private string? SelectSupportedContentCoding(HeaderStringValues headerValues)
    {
        var supportedEncodingName = default(string);
        var supportedEncodingQuality = QualityValues.MinValue;

        foreach (var headerValue in headerValues)
        {
            if (headerValue is null)
            {
                continue;
            }

            var headerValueTokens = new StringTokenizer(headerValue, s_codingTokenSeparators);

            foreach (var headerValueToken in headerValueTokens)
            {
                if (StringWithQualityHeaderValue.TryParse(headerValueToken.Value, out var headerValueObject))
                {
                    var currentEncodingName = headerValueObject.Value;
                    var currentEncodingQuality = default(double);

                    if (_compressionProviderRegistry.TryGetProvider(currentEncodingName, out var compressionProvider))
                    {
                        currentEncodingQuality = CreateCanonicalQualityValue(headerValueObject.Quality);

                        if (currentEncodingQuality > supportedEncodingQuality)
                        {
                            supportedEncodingName = compressionProvider?.EncodingName;
                            supportedEncodingQuality = currentEncodingQuality;
                        }
                    }
                    else if (ContentCodingTokens.IsAsterisk(currentEncodingName))
                    {
                        currentEncodingQuality = CreateCanonicalQualityValue(headerValueObject.Quality);

                        if (currentEncodingQuality > supportedEncodingQuality)
                        {
                            supportedEncodingName = _compressionOptions.EncodingName;
                            supportedEncodingQuality = currentEncodingQuality;
                        }
                    }
                }

                if (supportedEncodingQuality >= QualityValues.MaxValue)
                {
                    return supportedEncodingName;
                }
            }
        }

        return supportedEncodingName;
    }

    protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplySelectedContentCoding(request);

        if (request.Options.TryGetValue(RequestCompressionOptionKeys.EncodingContext, out var encodingContext))
        {
            return Send(request, encodingContext, cancellationToken);
        }
        else
        {
            return base.Send(request, cancellationToken);
        }
    }

    private HttpResponseMessage Send(HttpRequestMessage request, RequestCompressionEncodingContext? encodingContext, CancellationToken cancellationToken)
    {
        var response = base.Send(request, cancellationToken);

        FindSupportedContentCoding(request, response, encodingContext);

        return response;
    }

    protected sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ApplySelectedContentCoding(request);

        if (request.Options.TryGetValue(RequestCompressionOptionKeys.EncodingContext, out var encodingContext))
        {
            return SendAsync(request, encodingContext, cancellationToken);
        }
        else
        {
            return base.SendAsync(request, cancellationToken);
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, RequestCompressionEncodingContext? encodingContext, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        FindSupportedContentCoding(request, response, encodingContext);

        return response;
    }
}
