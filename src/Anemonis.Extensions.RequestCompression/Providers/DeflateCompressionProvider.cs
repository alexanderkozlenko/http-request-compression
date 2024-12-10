// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression.Providers;

internal sealed class DeflateCompressionProvider : HttpCompressionProvider
{
    public override Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
    {
        return new ZLibStream(outputStream, compressionLevel, true);
    }
}
