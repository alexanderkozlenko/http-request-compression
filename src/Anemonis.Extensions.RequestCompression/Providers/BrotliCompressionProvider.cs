// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;
using System.Runtime.Versioning;

namespace Anemonis.Extensions.RequestCompression.Providers;

[UnsupportedOSPlatform("browser")]
internal sealed class BrotliCompressionProvider : HttpCompressionProvider
{
    public override Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
    {
        return new BrotliStream(outputStream, compressionLevel, true);
    }
}
