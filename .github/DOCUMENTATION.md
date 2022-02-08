## Project Details

- The content codings supported by default:
  | Name | Description |
  | :--- | --- |
  | `br` | Brotli compressed data format |
  | `gzip` | GZIP file format |
- The media types compressed by default:
  | Name | Description |
  | :--- | --- |
  | `application/xml` | XML file formatt |
  | `application/json` | JSON file format |
- The logging events available for diagnostics:
  | ID | Level | Description |
  | :---: | :---: | --- |
  | 1 | Debug | Adding compression with the specified format |
- The default compression format is Brotli with fastest compression level.
- Compression can be disabled/enabled per a request with extension methods for request options.
- The handler can be used in a tandem with the [ASP.NET Core request decompression middleware](https://github.com/alexanderkozlenko/aspnetcore-request-decompression).

The default compression format is Brotli with fastest compression level.

The default set of Content-Type media types to compress:
- `application/xml` - XML file format
- `application/json` - JSON file format

## Code Examples

```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
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
    .AddRequestCompressionHandler("gzip", compressionLevel: CompressionLevel.Optimal);
```
```cs
services
    .AddRequestCompression();

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler("gzip", mediaTypes: new[] { "text/plain" });
```
```cs
services
    .AddRequestCompression(options =>
    {
        options.DefaultEncodingName = "gzip";
        options.DefaultCompressionLevel = CompressionLevel.Optimal;
        options.DefaultMediaTypes.Add("text/plain");
    });

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
```cs
request.Content = JsonContent.Create("Hello World!");
request.Options.DisableCompression();
```
```cs
request.Content = new StringContent("Hello World!");
request.Options.EnableCompression();
```
