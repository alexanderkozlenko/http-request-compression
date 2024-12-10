// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression.Providers;

internal sealed class GzipCompressionProvider : HttpCompressionProvider
{
    public override Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
    {
        return new GZipStream(outputStream, compressionLevel, true);
    }
}
