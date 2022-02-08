## Project Details

- The content codings supported by default:
  - `br` - Brotli compressed data format
  - `gzip` - GZIP file format
- The media types compressed by default:
  - `application/json`
  - `application/xml`
- The compression format by default is Brotli with the fastest compression level.
- The handler can be used in a tandem with the [ASP.NET Core request decompression middleware](https://github.com/alexanderkozlenko/aspnetcore-request-decompression).

## Code Examples

Default global configuration:
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
Custom global configuration:
```cs
services
    .AddRequestCompression(options =>
    {
        options.DefaultEncodingName = "gzip";
        options.DefaultMediaTypes.Add("text/plain");
    });

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
Custom per-client configuration:
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler("gzip");
```
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler("gzip", mediaTypes: new[] { "text/plain" });
```
Custom per-request configuration:
```cs
request.Content = JsonContent.Create("Hello World!");
request.SetCompressionEnabled(false);
```
```cs
request.Content = new StringContent("Hello World!");
request.SetCompressionEnabled(true);
```
