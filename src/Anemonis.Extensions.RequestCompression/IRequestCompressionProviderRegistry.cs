// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public interface IRequestCompressionProviderRegistry
{
    bool TryGetProvider(string encodingName, out IRequestCompressionProvider? provider);
}
