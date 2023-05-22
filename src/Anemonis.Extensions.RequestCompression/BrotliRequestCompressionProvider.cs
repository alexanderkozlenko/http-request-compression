// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public sealed class BrotliRequestCompressionProvider : IRequestCompressionProvider
{
    public BrotliRequestCompressionProvider()
    {
    }

    public Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(outputStreeam);

        return new BrotliStream(outputStreeam, compressionLevel, leaveOpen: true);
    }

    public string EncodingName
    {
        get
        {
            return "br";
        }
    }
}
