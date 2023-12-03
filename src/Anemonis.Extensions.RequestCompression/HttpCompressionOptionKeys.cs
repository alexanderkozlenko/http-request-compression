// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Anemonis.Extensions.RequestCompression;

/// <summary>Provides the keys in the options collection for HTTP request compression operations. This class cannot be inherited.</summary>
public static class HttpCompressionOptionKeys
{
    /// <summary>Represents the key to get or set a compression context for an HTTP request.</summary>
    public static readonly HttpRequestOptionsKey<HttpCompressionContext?> HttpCompressionContext = new("HttpCompressionContext");
}
