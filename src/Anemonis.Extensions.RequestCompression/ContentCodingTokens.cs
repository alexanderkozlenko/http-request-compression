// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1802

using System.Diagnostics.CodeAnalysis;

namespace Anemonis.Extensions.RequestCompression;

internal static class ContentCodingTokens
{
    public static readonly string Identity = "identity";

    public static bool IsIdentity([NotNullWhen(true)] string? name)
    {
        return string.Equals(name, Identity, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAsterisk([NotNullWhen(true)] string? name)
    {
        return (name is { Length: 1 }) && (name[0] == '*');
    }
}
