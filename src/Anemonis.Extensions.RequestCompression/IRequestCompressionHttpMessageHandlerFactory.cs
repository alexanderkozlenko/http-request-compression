// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public interface IRequestCompressionHttpMessageHandlerFactory
{
    DelegatingHandler CreateHandler(string? encodingName, CompressionLevel? compressionLevel, IEnumerable<string>? mediaTypes);
}
