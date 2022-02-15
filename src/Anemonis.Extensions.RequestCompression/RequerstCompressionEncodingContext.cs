// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequerstCompressionEncodingContext
{
    public RequerstCompressionEncodingContext()
    {
    }

    /// <summary>Gets or sets the encoding token that defines a preferrable compression format supported by client and server.</summary>
    public string? EncodingName
    {
        get;
        set;
    }
}
