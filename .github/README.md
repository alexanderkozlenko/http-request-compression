## HTTP client handler for request compression

Transparent HTTP client message handler for request compression with support for RFC 7694.

|ID|Stable|Preview|
|:-|:-|:-|
|Anemonis.Extensions.RequestCompression|[![NuGet](https://img.shields.io/nuget/v/Anemonis.Extensions.RequestCompression?style=flat-square)](https://nuget.org/packages/Anemonis.Extensions.RequestCompression)|[![NuGet](https://img.shields.io/nuget/vpre/Anemonis.Extensions.RequestCompression?style=flat-square)](https://nuget.org/packages/Anemonis.Extensions.RequestCompression)|

### Features

- The content codings supported by default are Brotli and GZIP.
- The media types compressed by default are `application/json` and `application/xml`.
- The compression format by default is Brotli with the fastest compression level.
- The middleware supports RFC 7694 for OPTIONS requests.

### Quick Links

- [IETF - RFC 7694](https://www.rfc-editor.org/rfc/rfc7694)
- [IETF - RFC 9110](https://www.rfc-editor.org/rfc/rfc9110)

### Quick Start

An example of a default global configuration:

```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```

An example of a custom global configuration:

```cs
services
    .AddRequestCompression(options =>
    {
        options.EncodingName = "gzip";
        options.MediaTypes.Add("text/plain");
    });

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```

An example of a custom per-handler configuration:

```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler(options =>
    {
        options.EncodingName = "gzip";
        options.MediaTypes.Add("text/plain");
    });
```

An example of a custom per-request configuration:

```cs
var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/items");

request.Content = JsonContent.Create(item);
request.Options.SetCompressionEnabled(false);

await httpClient.SendAsync(request);
```

An example of discovery for a compression format supported by server (RFC 7694):

```cs
var request1 = new HttpRequestMessage(HttpMethod.Options, "/api/v1.0/items");
var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/items");

request1.Options.AddCompressionDiscovery(out var encodingContext);

await httpClient.SendAsync(request1);

request2.Content = JsonContent.Create(item);
request2.Options.SetCompressionEncoding(encodingContext.EncodingName);

await httpClient.SendAsync(request2);
```