// © Oleksandr Kozlenko. Licensed under the MIT license.

using System.Net.Mime;

namespace Anemonis.Extensions.RequestCompression;

internal static class RequestCompressionDefaults
{
    public static HashSet<string> MimeTypes = new(2, StringComparer.OrdinalIgnoreCase)
    {
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Application.Json,
    };
}
