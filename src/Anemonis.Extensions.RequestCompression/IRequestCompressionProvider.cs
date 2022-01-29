// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public interface IRequestCompressionProvider
{
    Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel);

    string EncodingName
    {
        get;
    }
}
