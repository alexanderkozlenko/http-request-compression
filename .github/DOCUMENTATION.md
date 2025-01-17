## HTTP request compression - Overview

<p />

### About

<p />

The library includes built-in compression providers for Brotli, DEFLATE, and GZIP formats. With the default options, any HTTP request content of the "application/json" media type will be compressed with Brotli format using the fastest level of compression. Use OPTIONS request method with an HTTP compression context to discover an available compression encoding supported by a server.

<p />

> [!NOTE]
> The built-in Brotli compression provider is not available for ASP.NET Core Blazor WebAssembly (WASM).

<p />

> [!NOTE]
> The built-in DEFLATE compression provider uses ZLIB compression format.

<p />

### How to Use

<p />

How to add the compression handler to a named HTTP client with default settings:

<p />

```cs
services.AddHttpClient("my-client")
        .AddCompressionHandler();
```

<p />

How to add the compression handler to a named HTTP client with specific settings:

<p />

```cs
services.AddHttpClient("my-client")
        .AddCompressionHandler(options =>
        {
            options.MediaTypes.Add("application/json-seq");
            options.CompressionEncoding = "deflate";
            options.CompressionLevel = CompressionLevel.Optimal;
        });
```

<p />

How to add the compression handler to a named HTTP client with specific shared settings:

<p />

```cs
services.Configure<HttpCompressionOptions>(options =>
        {
            options.MediaTypes.Add("application/json-seq");
            options.CompressionEncoding = "deflate";
            options.CompressionLevel = CompressionLevel.Optimal;
        });

services.AddHttpClient("my-client")
        .AddCompressionHandler();
```

<p />

How to discover and apply a compression encoding supported by a server:

<p />

```cs
var context = new HttpCompressionContext();
var request1 = new HttpRequestMessage(HttpMethod.Options, "/api/items");

request1.Options.Set(HttpCompressionOptionKeys.HttpCompressionContext, context);

await httpClient.SendAsync(request1);

var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/items");

request2.Content = JsonContent.Create(item);
request1.Options.Set(HttpCompressionOptionKeys.HttpCompressionContext, context);

await httpClient.SendAsync(request2);
```

<p />

The compression encoding discovery support can be added for a server using the build-in middleware:

<p />

```cs
app.Use((context, next) =>
{
    if (string.Equals(context.Request.Method, HttpMethods.Options))
    {
        var options = app.Services.GetService<IOptions<RequestDecompressionOptions>>();

        if (options is not null)
        {
            var encodingNames = options.Value.DecompressionProviders.Keys;

            context.Response.Headers.AcceptEncoding = string.Join(',', encodingNames);
        }
    }

    return next(context);
});

app.Run();
```

<p />

### References

- [RFC 9110 - HTTP Semantics](https://datatracker.ietf.org/doc/html/rfc9110)
- [ASP.NET Core - Request decompression in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/request-decompression)
