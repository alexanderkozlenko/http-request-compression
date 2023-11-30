// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Frozen;
using System.IO.Compression;
using System.Net.Mime;
using Anemonis.Extensions.RequestCompression.Providers;

namespace Anemonis.Extensions.RequestCompression;

internal static class HttpCompressionDefaults
{
    public const string DefaultCompressionEncoding = "br";
    public const CompressionLevel DefaultCompressionLevel = CompressionLevel.Fastest;

    public static readonly FrozenDictionary<string, HttpCompressionProvider> DefaultCompressionProviders = CreateCompressionProviders().ToFrozenDictionary();
    public static readonly FrozenSet<string> DefaultMediaTypes = CreateMediaTypes().ToFrozenSet();

    private static Dictionary<string, HttpCompressionProvider> CreateCompressionProviders()
    {
        return new(StringComparer.OrdinalIgnoreCase)
        {
            ["br"] = new BrotliCompressionProvider(),
            ["deflate"] = new DeflateCompressionProvider(),
            ["gzip"] = new GzipCompressionProvider(),
        };
    }

    private static HashSet<string> CreateMediaTypes()
    {
        return new(StringComparer.OrdinalIgnoreCase)
        {
            MediaTypeNames.Application.Json,
        };
    }
}
