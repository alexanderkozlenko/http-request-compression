// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionProviderRegistry : IRequestCompressionProviderRegistry
{
    private readonly IOptions<RequestCompressionOptions> _options;

    private volatile Dictionary<string, IRequestCompressionProvider?>? _providers;

    public RequestCompressionProviderRegistry(IOptions<RequestCompressionOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    private static Dictionary<string, IRequestCompressionProvider?> CreateProviders(ICollection<Type> providerTypes)
    {
        var providers = new Dictionary<string, IRequestCompressionProvider?>(providerTypes.Count + 1, StringComparer.OrdinalIgnoreCase)
        {
            [ContentCodingTokens.Identity] = default,
        };

        foreach (var providerType in providerTypes)
        {
            var provider = (IRequestCompressionProvider)Activator.CreateInstance(providerType)!;
            var encodingName = provider.EncodingName;

            if ((encodingName is not { Length: > 0 }) || !ContentCodingTokenIsSupported(encodingName))
            {
                throw new InvalidOperationException($"The encoding name for provider '{providerType}' is invalid.");
            }

            providers[encodingName] = provider;
        }

        return providers;
    }

    private static bool ContentCodingTokenIsSupported(string encodingName)
    {
        return
            !string.Equals(encodingName, ContentCodingTokens.Identity, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(encodingName, ContentCodingTokens.Asterisk, StringComparison.Ordinal);
    }

    public bool TryGetProvider(string encodingName, out IRequestCompressionProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(encodingName);

        _providers ??= CreateProviders(_options.Value.Providers);

        return _providers.TryGetValue(encodingName, out provider);
    }
}
