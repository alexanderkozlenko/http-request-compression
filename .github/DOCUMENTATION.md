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
var request = new HttpRequestMessage(HttpMethod.Post, "/resource");

request.Content = new StringContent("Hello World!");
request.Options.SetCompressionEnabled(true);

await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
```
Discovery of compression format supported by server (RFC 7694):
```cs
var request1 = new HttpRequestMessage(HttpMethod.Options, "/resource");
var request2 = new HttpRequestMessage(HttpMethod.Post, "/resource");

request1.Options.AddCompressionDiscovery(out var encodingContext);

await httpClient.SendAsync(request1, cancellationToken).ConfigureAwait(false);

request2.Content = JsonContent.Create("Hello World!");
request2.Options.SetCompressionEncoding(encodingContext.EncodingName);

await httpClient.SendAsync(request2, cancellationToken).ConfigureAwait(false);
```
