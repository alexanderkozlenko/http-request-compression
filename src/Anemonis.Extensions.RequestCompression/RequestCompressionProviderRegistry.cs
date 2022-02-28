// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionProviderRegistry : IRequestCompressionProviderRegistry
{
    private readonly IOptions<RequestCompressionOptions> _options;

    private volatile Dictionary<string, IRequestCompressionProvider?>? _compressionProviders;

    public RequestCompressionProviderRegistry(IOptions<RequestCompressionOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    private static Dictionary<string, IRequestCompressionProvider?> CreateCompressionProviders(RequestCompressionProviderCollection compressionProviderTypes)
    {
        var compressionProvidersCount = compressionProviderTypes.Count + 1;

        var compressionProviders = new Dictionary<string, IRequestCompressionProvider?>(compressionProvidersCount, StringComparer.OrdinalIgnoreCase)
        {
            [ContentCodingTokens.Identity] = default,
        };

        foreach (var compressionProviderType in compressionProviderTypes)
        {
            var compressionProvider = (IRequestCompressionProvider)Activator.CreateInstance(compressionProviderType)!;
            var encodingName = compressionProvider.EncodingName;

            if ((encodingName is not { Length: > 0 }) || !ContentCodingTokenIsSupported(encodingName))
            {
                throw new InvalidOperationException($"The encoding name for provider '{compressionProviderType}' is invalid.");
            }

            compressionProviders[encodingName] = compressionProvider;
        }

        return compressionProviders;
    }

    private static bool ContentCodingTokenIsSupported(string encodingName)
    {
        return !string.Equals(encodingName, ContentCodingTokens.Identity, StringComparison.OrdinalIgnoreCase) && !ContentCodingTokens.IsAsterisk(encodingName);
    }

    public bool TryGetProvider(string encodingName, out IRequestCompressionProvider? compressionProvider)
    {
        ArgumentNullException.ThrowIfNull(encodingName);

        _compressionProviders ??= CreateCompressionProviders(_options.Value.Providers);

        return _compressionProviders.TryGetValue(encodingName, out compressionProvider);
    }
}
