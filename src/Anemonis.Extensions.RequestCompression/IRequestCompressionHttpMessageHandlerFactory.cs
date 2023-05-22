// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

namespace Anemonis.Extensions.RequestCompression;

public interface IRequestCompressionHttpMessageHandlerFactory
{
    DelegatingHandler CreateHandler(string name);
}
