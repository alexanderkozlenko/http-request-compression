// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1802

namespace Anemonis.Extensions.RequestCompression;

internal static class ContentCodingTokens
{
    public static readonly string Identity = "identity";

    public static bool IsAsterisk(string name)
    {
        return (name.Length == 1) && (name[0] == '*');
    }
}
