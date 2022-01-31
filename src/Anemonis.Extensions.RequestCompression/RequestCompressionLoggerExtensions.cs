// © Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Anemonis.Extensions.RequestCompression;

internal static partial class RequestCompressionLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 1,
        EventName = "AddingCompression",
        Message = "Adding compression with '{format}' format")]
    public static partial void AddingCompression(this ILogger logger, string format);
}
