// © Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;
using System.Net;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class CodingStreamContent : HttpContent
{
    private readonly HttpContent _originalContent;
    private readonly IRequestCompressionProvider _compressionProvider;
    private readonly CompressionLevel _compressionLevel;

    public CodingStreamContent(HttpContent originalContent, IRequestCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(originalContent);
        ArgumentNullException.ThrowIfNull(compressionProvider);

        _originalContent = originalContent;
        _compressionProvider = compressionProvider;
        _compressionLevel = compressionLevel;
    }

    protected sealed override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var inputStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        if (ReferenceEquals(stream, inputStream) || (inputStream is not { CanWrite: true }))
        {
            throw new InvalidOperationException($"The compression stream created for encoding '{_compressionProvider.EncodingName}' is invalid.");
        }

        await using (inputStream.ConfigureAwait(false))
        {
            await _originalContent.CopyToAsync(inputStream, context, cancellationToken).ConfigureAwait(false);
            await inputStream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    protected sealed override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return SerializeToStreamAsync(stream, context, default);
    }

    protected sealed override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var inputStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        if (ReferenceEquals(stream, inputStream) || (inputStream is not { CanWrite: true }))
        {
            throw new InvalidOperationException($"The compression stream created for encoding '{_compressionProvider.EncodingName}' is invalid.");
        }

        using (inputStream)
        {
            _originalContent.CopyTo(inputStream, context, cancellationToken);
            inputStream.Flush();
        }
    }

    protected sealed override bool TryComputeLength(out long length)
    {
        length = 0L;

        return false;
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _originalContent.Dispose();
        }

        base.Dispose(disposing);
    }
}
