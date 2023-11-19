// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class HttpCompressionHandler : DelegatingHandler
{
    private static readonly char[] s_headerSeparators = [','];

    private readonly Dictionary<string, HttpCompressionProvider> _compressionProviders;
    private readonly HashSet<string> _mediaTypes;
    private readonly string _compressionEncoding;
    private readonly CompressionLevel _compressionLevel;

    public HttpCompressionHandler(Dictionary<string, HttpCompressionProvider> compressionProviders, HashSet<string> mediaTypes, string compressionEncoding, CompressionLevel compressionLevel)
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
            if (request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var context) && (context is not null))
            {
                var response = base.Send(request, cancellationToken);

                TryFindAcceptableContentEncoding(response, context);

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
                request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var context);

                TryEncodeContent(request, context);
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
            if (request.Options.TryGetValue(HttpCompressionOptionKeys.HttpCompressionContext, out var context) && (context is not null))
            {
                return SendCoreAsync(request, context, cancellationToken);
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

                TryEncodeContent(request, context);
            }

            return base.SendAsync(request, cancellationToken);
        }

        async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, HttpCompressionContext context, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            TryFindAcceptableContentEncoding(response, context);

            return response;
        }
    }

    private void TryEncodeContent(HttpRequestMessage request, HttpCompressionContext? context)
    {
        var contentEncoding = _compressionEncoding;

        if (context is not null)
        {
            contentEncoding = context.CompressionEncoding;
        }

        if (string.IsNullOrEmpty(contentEncoding) || string.Equals(contentEncoding, "identity", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!_compressionProviders.TryGetValue(contentEncoding, out var compressionProvider))
        {
            return;
        }

        var mediaType = request.Content!.Headers.ContentType?.MediaType;

        if (!string.IsNullOrEmpty(mediaType) && _mediaTypes.Contains(mediaType))
        {
            request.Content = CreateEncodedContent(request.Content, contentEncoding, compressionProvider, _compressionLevel);
        }
    }

    private void TryFindAcceptableContentEncoding(HttpResponseMessage response, HttpCompressionContext context)
    {
        if (!response.IsSuccessStatusCode || !response.Headers.NonValidated.TryGetValues("Accept-Encoding", out var headerValues))
        {
            return;
        }

        if (TryGetContentEncoding(headerValues, _compressionProviders, out var contentEncoding))
        {
            context.CompressionEncoding = contentEncoding;
        }
    }

    private static HttpCompressionContent CreateEncodedContent(HttpContent source, string contentEncoding, HttpCompressionProvider provider, CompressionLevel level)
    {
        var result = new HttpCompressionContent(source, provider, level);

        foreach (var (headerName, headerValues) in source.Headers.NonValidated)
        {
            result.Headers.TryAddWithoutValidation(headerName, headerValues);
        }

        result.Headers.ContentEncoding.Add(contentEncoding);
        result.Headers.ContentLength = null;

        return result;
    }

    private static bool TryGetContentEncoding(HeaderStringValues headerValues, Dictionary<string, HttpCompressionProvider> compressionProviders, [NotNullWhen(true)] out string? contentEncoding)
    {
        var bestContentEncoding = default(string);
        var bestContentEncodingQuality = .0;

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

                var currentContentEncodingQuality = headerValueWithQuailty.Quality.HasValue ? headerValueWithQuailty.Quality.Value : 1;

                if (currentContentEncodingQuality <= bestContentEncodingQuality)
                {
                    continue;
                }

                var currentContentEncoding = headerValueWithQuailty.Value;

                if (string.Equals(currentContentEncoding, "*", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(currentContentEncoding, "identity", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (compressionProviders.ContainsKey(currentContentEncoding))
                {
                    bestContentEncoding = currentContentEncoding;
                    bestContentEncodingQuality = currentContentEncodingQuality;

                    if (bestContentEncodingQuality >= 1)
                    {
                        break;
                    }
                }
            }
        }

        contentEncoding = bestContentEncoding;

        return contentEncoding is not null;
    }
}
