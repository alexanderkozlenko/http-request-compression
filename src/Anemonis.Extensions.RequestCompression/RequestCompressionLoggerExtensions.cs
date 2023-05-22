// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Anemonis.Extensions.RequestCompression;

internal static partial class RequestCompressionLoggerExtensions
{
    [LoggerMessage(1, LogLevel.Debug, "The request will be compressed with '{provider}'.", EventName = "CompressWith")]
    public static partial void CompressingWith(this ILogger logger, string provider);
}
