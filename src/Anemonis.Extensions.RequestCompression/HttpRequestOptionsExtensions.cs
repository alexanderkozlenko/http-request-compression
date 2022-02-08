// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public static class HttpRequestOptionsExtensions
{
    /// <summary>Turns on request compression by <see cref="RequestCompressionHttpMessageHandler" /> regardles of content type.</summary>
    public static void EnableCompression(this HttpRequestOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionsKey, true);
    }

    /// <summary>Turns off request compression by <see cref="RequestCompressionHttpMessageHandler" /> regardles of content type.</summary>
    public static void DisableCompression(this HttpRequestOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionsKey, false);
    }
}
