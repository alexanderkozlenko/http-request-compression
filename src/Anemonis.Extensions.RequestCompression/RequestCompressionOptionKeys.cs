// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression;

public static class RequestCompressionOptionKeys
{
    public static readonly HttpRequestOptionsKey<bool> CompressionEnabled =
        new($"{typeof(RequestCompressionOptionKeys).Namespace}.{nameof(CompressionEnabled)}");

    public static readonly HttpRequestOptionsKey<string?> EncodingName =
        new($"{typeof(RequestCompressionOptionKeys).Namespace}.{nameof(EncodingName)}");

    public static readonly HttpRequestOptionsKey<CompressionLevel> CompressionLevel =
        new($"{typeof(RequestCompressionOptionKeys).Namespace}.{nameof(CompressionLevel)}");

    public static readonly HttpRequestOptionsKey<RequerstCompressionEncodingContext?> EncodingContext =
        new($"{typeof(RequestCompressionOptionKeys).Namespace}.{nameof(EncodingContext)}");
}
