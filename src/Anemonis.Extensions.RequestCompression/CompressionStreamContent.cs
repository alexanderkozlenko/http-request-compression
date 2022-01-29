// © Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;
using System.Net;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class CompressionStreamContent : HttpContent
{
    private readonly HttpContent _originalContent;
    private readonly IRequestCompressionProvider _compressionProvider;
    private readonly CompressionLevel _compressionLevel;

    public CompressionStreamContent(HttpContent originalContent, IRequestCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        _originalContent = originalContent;
        _compressionProvider = compressionProvider;
        _compressionLevel = compressionLevel;
    }

    protected sealed override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        var compressionStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        await using (compressionStream.ConfigureAwait(false))
        {
            await _originalContent.CopyToAsync(compressionStream, context, cancellationToken).ConfigureAwait(false);
            await compressionStream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    protected sealed override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        return SerializeToStreamAsync(stream, context, default);
    }

    protected sealed override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        var compressionStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        using (compressionStream)
        {
            _originalContent.CopyTo(compressionStream, context, cancellationToken);
            compressionStream.Flush();
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
