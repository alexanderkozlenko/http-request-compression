// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Defines the core behavior of creating a compression stream and provides a base for derived classes.</summary>
public abstract class HttpCompressionProvider
{
    /// <summary>Initializes a new instance of the <see cref="HttpCompressionProvider" /> class.</summary>
    protected HttpCompressionProvider()
    {
    }

    /// <summary>When overridden in a derived class, creates a compression stream which writes the compressed content to the underlying stream.</summary>
    /// <param name="outputStreeam">Teh stream to write the compressed content to.</param>
    /// <param name="compressionLevel">The level of compression to use.</param>
    /// <returns>A writable stream.</returns>
    public abstract Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel);
}
