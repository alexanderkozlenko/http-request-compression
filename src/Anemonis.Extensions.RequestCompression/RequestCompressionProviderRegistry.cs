// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using Microsoft.Extensions.Options;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionProviderRegistry : IRequestCompressionProviderRegistry
{
    private readonly Func<Dictionary<string, IRequestCompressionProvider>> _providersFactory;
    private readonly IOptions<RequestCompressionOptions> _options;

    private Dictionary<string, IRequestCompressionProvider>? _providers;

    public RequestCompressionProviderRegistry(IOptions<RequestCompressionOptions> options)
    {
        _providersFactory = CreateProviders;
        _options = options;
    }

    public IRequestCompressionProvider GetProvider(string encodingName)
    {
        ArgumentNullException.ThrowIfNull(encodingName);

        var providers = LazyInitializer.EnsureInitialized(ref _providers, _providersFactory);

        if (!providers.TryGetValue(encodingName, out var provider))
        {
            throw new InvalidOperationException($"No matching request compression provider found for encoding '{encodingName}'.");
        }

        return provider;
    }

    private Dictionary<string, IRequestCompressionProvider> CreateProviders()
    {
        var providerTypes = _options.Value.Providers;
        var providers = new Dictionary<string, IRequestCompressionProvider>(providerTypes.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var providerType in providerTypes)
        {
            var provider = (IRequestCompressionProvider)Activator.CreateInstance(providerType)!;

            providers[provider.EncodingName] = provider;
        }

        return providers;
    }
}
