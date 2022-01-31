## Project Details

The default set of Content-Encoding HTTP content codings supported:
- `br` - Brotli compressed data format
- `gzip` - GZIP file format

The default compression format is Brotli with fastest compression level.

The default set of Content-Type MIME types to compress:
- `application/xml` - XML file format
- `application/json` - JSON file format

The handler can be used in a tandem with the [ASP.NET Core request decompression middleware](https://github.com/alexanderkozlenko/aspnetcore-request-decompression).

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
    .AddRequestCompressionHandler("gzip", mimeTypes: new[] { "text/plain" });
```
```cs
services
    .AddRequestCompression(options =>
    {
        options.DefaultEncodingName = "gzip";
        options.DefaultCompressionLevel = CompressionLevel.Optimal;
        options.DefaultMimeTypes.Add("text/plain");
    });

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
