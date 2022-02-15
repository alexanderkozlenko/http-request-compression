namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class DelegatingPrimaryHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

    public DelegatingPrimaryHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    protected sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _handler.Invoke(request);
    }
}
