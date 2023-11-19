// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression.Providers;

internal sealed class DeflateCompressionProvider : HttpCompressionProvider
{
    public override Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel)
    {
        return new ZLibStream(outputStreeam, compressionLevel, leaveOpen: true);
    }
}
