// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionHttpMessageHandler : DelegatingHandler
{
    private readonly IRequestCompressionProvider _compressionProvider;
    private readonly CompressionLevel _compressionLevel;

    public RequestCompressionHttpMessageHandler(IRequestCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        _compressionProvider = compressionProvider;
        _compressionLevel = compressionLevel;
    }

    protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request?.Content is { } originalContent)
        {
            request.Content = CreateCompressionStreamContent(originalContent);
        }

        return base.Send(request!, cancellationToken);
    }

    protected sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request?.Content is { } originalContent)
        {
            request.Content = CreateCompressionStreamContent(originalContent);
        }

        return base.SendAsync(request!, cancellationToken);
    }

    private HttpContent CreateCompressionStreamContent(HttpContent originalContent)
    {
        var compressionContent = new CompressionStreamContent(originalContent, _compressionProvider, _compressionLevel);

        foreach (var (name, values) in originalContent.Headers.NonValidated)
        {
            compressionContent.Headers.TryAddWithoutValidation(name, values);
        }

        compressionContent.Headers.ContentEncoding.Add(_compressionProvider.EncodingName);
        compressionContent.Headers.ContentLength = null;

        return compressionContent;
    }
}
