// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public sealed class GzipCompressionProvider : IRequestCompressionProvider
{
    public GzipCompressionProvider()
    {
    }

    public Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(outputStreeam);

        return new GZipStream(outputStreeam, compressionLevel, leaveOpen: true);
    }

    public string EncodingName
    {
        get
        {
            return "gzip";
        }
    }
}
