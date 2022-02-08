// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1573
#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public static class HttpRequestMessageExtensions
{
    /// <summary>Toggles request compression in <see cref="RequestCompressionHttpMessageHandler" /> with the highest priority.</summary>
    /// <param name="compressionEnabled"><see langword="true" /> if compression is enabled; otherwise <see langword="false" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
    public static void SetCompressionEnabled(this HttpRequestMessage request, bool compressionEnabled)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Options.Set(RequestCompressionHttpMessageHandler.EnableCompressionOptionsKey, compressionEnabled);
    }
}
