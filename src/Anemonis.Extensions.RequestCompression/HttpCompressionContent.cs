// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net;

namespace Anemonis.Extensions.RequestCompression;

internal sealed class HttpCompressionContent : HttpContent
{
    private readonly HttpContent _originalContent;
    private readonly HttpCompressionProvider _compressionProvider;
    private readonly CompressionLevel _compressionLevel;

    public HttpCompressionContent(HttpContent originalContent, HttpCompressionProvider compressionProvider, CompressionLevel compressionLevel)
    {
        _originalContent = originalContent;
        _compressionProvider = compressionProvider;
        _compressionLevel = compressionLevel;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _originalContent.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        var compressionStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        if (compressionStream is not { CanWrite: true })
        {
            ThrowUnwritableStreamException();
        }

        try
        {
            _originalContent.CopyTo(compressionStream, context, cancellationToken);
        }
        finally
        {
            if (!ReferenceEquals(compressionStream, stream))
            {
                compressionStream.Flush();
                compressionStream.Dispose();
            }
        }
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        var compressionStream = _compressionProvider.CreateStream(stream, _compressionLevel);

        if (compressionStream is not { CanWrite: true })
        {
            ThrowUnwritableStreamException();
        }

        try
        {
            await _originalContent.CopyToAsync(compressionStream, context, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (!ReferenceEquals(compressionStream, stream))
            {
                await compressionStream.FlushAsync(cancellationToken).ConfigureAwait(false);
                await compressionStream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        return SerializeToStreamAsync(stream, context, default);
    }

    protected override bool TryComputeLength(out long length)
    {
        length = default;

        return false;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnwritableStreamException()
    {
        throw new ArgumentException("The stream does not support writing.");
    }
}
