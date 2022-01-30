## Project Details

Primary HTTP client handler features:

- The handler supports requests with chained encodings.
- The handler supports custom encoding providers for any encoding.

When added with default providers, a handler can be added for these encodings:

| Encoding Token | Encoding Description |
| --- | --- |
| `br` | Brotli compressed data format |
| `gzip` | GZIP file format |

The handler specifies a default set of MIME types for compression:
- `application/xml`
- `application/json`

## Code Examples

```cs
services
    .AddRequestCompression();

services
    .AddHttpClient<IMyService, MyService>()
    .AddRequestCompressionHandler();
```
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient<IMyService, MyService>()
    .AddRequestCompressionHandler("gzip", CompressionLevel.SmallestSize);
```
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient<IMyService, MyService>()
    .AddRequestCompressionHandler("gzip", CompressionLevel.SmallestSize, new[] { "text/plain" });
```
