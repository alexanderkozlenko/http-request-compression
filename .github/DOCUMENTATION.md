## Project Details

- The content codings supported by default:
  - `br` - Brotli compressed data format
  - `gzip` - GZIP file format
- The media types compressed by default:
  - `application/json`
  - `application/xml`
- The compression format by default is Brotli with the fastest compression level.
- The middleware supports RFC 7694 for OPTIONS requests.
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
        options.EncodingName = "gzip";
        options.MediaTypes.Add("text/plain");
    });

services
    .AddHttpClient(Options.DefaultName)
    .AddRequestCompressionHandler();
```
Custom per-handler configuration:
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
Custom per-request configuration:
```cs
var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/items");

request.Content = JsonContent.Create(item);
request.Options.SetCompressionEnabled(false);

await httpClient.SendAsync(request);
```
Discovery of compression format supported by server (RFC 7694):
```cs
var request1 = new HttpRequestMessage(HttpMethod.Options, "/api/v1.0/items");
var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/items");

request1.Options.AddCompressionDiscovery(out var encodingContext);

await httpClient.SendAsync(request1);

request2.Content = JsonContent.Create(item);
request2.Options.SetCompressionEncoding(encodingContext.EncodingName);

await httpClient.SendAsync(request2);
```
