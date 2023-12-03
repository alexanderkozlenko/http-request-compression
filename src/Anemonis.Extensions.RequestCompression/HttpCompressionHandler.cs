// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class HttpCompressionHandler : DelegatingHandler
{
    private static readonly char[] s_headerSeparators = [','];

    private readonly FrozenDictionary<string, HttpCompressionProvider> _compressionProviders;
    private readonly FrozenSet<string> _mediaTypes;
    private readonly string _compressionEncoding;
    private readonly CompressionLevel _compressionLevel;

    public HttpCompressionHandler(FrozenDictionary<string, HttpCompressionProvider> compressionProviders, FrozenSet<string> mediaTypes, string compressionEncoding, CompressionLevel compressionLevel)
    {
        _compressionProviders = compressionProviders;
        _mediaTypes = mediaTypes;
        _compressionEncoding = compressionEncoding;
        _compressionLevel = compressionLevel;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return base.Send(request!, cancellationToken);
        }

        if (request.Method.Equals(HttpMethod.Options))
        {
            if (request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var compressionContext) && (compressionContext is not null))
            {
                var response = base.Send(request, cancellationToken);

                TryFindQualifiedEncoding(response, compressionContext);

                return response;
            }
            else
            {
                return base.Send(request, cancellationToken);
            }
        }
        else
        {
            if (request.Content is not null)
            {
                request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var compressionContext);

                TryCompressContent(request, compressionContext);
            }

            return base.Send(request, cancellationToken);
        }
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return base.SendAsync(request!, cancellationToken);
        }

        if (request.Method.Equals(HttpMethod.Options))
        {
            if (request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var compressionContext) && (compressionContext is not null))
            {
                return SendCoreAsync(request, compressionContext, cancellationToken);
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
        else
        {
            if (request.Content is not null)
            {
                request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var context);

                TryCompressContent(request, context);
            }

            return base.SendAsync(request, cancellationToken);
        }

        async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, HttpCompressionContext compressionContext, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            TryFindQualifiedEncoding(response, compressionContext);

            return response;
        }
    }

    private void TryCompressContent(HttpRequestMessage request, HttpCompressionContext? compressionContext)
    {
        var compressionEncoding = _compressionEncoding;

        if (compressionContext is not null)
        {
            compressionEncoding = compressionContext.CompressionEncoding;
        }

        if (string.IsNullOrEmpty(compressionEncoding) || string.Equals(compressionEncoding, "identity", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!_compressionProviders.TryGetValue(compressionEncoding, out var compressionProvider))
        {
            return;
        }

        var mediaType = request.Content!.Headers.ContentType?.MediaType;

        if (!string.IsNullOrEmpty(mediaType) && _mediaTypes.Contains(mediaType))
        {
            request.Content = CreateCompressionContent(request.Content, compressionEncoding, compressionProvider, _compressionLevel);
        }
    }

    private void TryFindQualifiedEncoding(HttpResponseMessage response, HttpCompressionContext compressionContext)
    {
        if (!response.IsSuccessStatusCode || !response.Headers.NonValidated.TryGetValues("Accept-Encoding", out var headerValues))
        {
            return;
        }

        if (TryGetQualifiedEncoding(headerValues, _compressionProviders, out var compressionEncoding))
        {
            compressionContext.CompressionEncoding = compressionEncoding;
        }
    }

    private static bool TryGetQualifiedEncoding(HeaderStringValues headerValues, FrozenDictionary<string, HttpCompressionProvider> compressionProviders, [NotNullWhen(true)] out string? compressionEncoding)
    {
        var bestEncoding = default(string);
        var bestEncodingQuality = .0;

        foreach (var headerValue in headerValues)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                continue;
            }

            var headerTokens = new StringTokenizer(headerValue, s_headerSeparators);

            foreach (var headerToken in headerTokens)
            {
                if (!StringWithQualityHeaderValue.TryParse(headerToken.Value, out var headerValueWithQuailty) || string.IsNullOrEmpty(headerValueWithQuailty.Value))
                {
                    continue;
                }

                var currentEncodingQuality = headerValueWithQuailty.Quality.HasValue ? headerValueWithQuailty.Quality.Value : 1;

                if (currentEncodingQuality <= bestEncodingQuality)
                {
                    continue;
                }

                var currentEncoding = headerValueWithQuailty.Value;

                if (string.Equals(currentEncoding, "*", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(currentEncoding, "identity", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (compressionProviders.ContainsKey(currentEncoding))
                {
                    bestEncoding = currentEncoding;
                    bestEncodingQuality = currentEncodingQuality;

                    if (bestEncodingQuality >= 1)
                    {
                        break;
                    }
                }
            }
        }

        compressionEncoding = bestEncoding;

        return compressionEncoding is not null;
    }

    private static HttpCompressionContent CreateCompressionContent(HttpContent originalContent, string compressionEncoding, HttpCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        var compressionContent = new HttpCompressionContent(originalContent, compressionProvider, compressionLevel);

        foreach (var (headerName, headerValues) in originalContent.Headers.NonValidated)
        {
            compressionContent.Headers.TryAddWithoutValidation(headerName, headerValues);
        }

        compressionContent.Headers.ContentEncoding.Add(compressionEncoding);
        compressionContent.Headers.ContentLength = null;

        return compressionContent;
    }
}
