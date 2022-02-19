namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class DelegatingPrimaryHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public DelegatingPrimaryHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected sealed override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _handler.Invoke(request);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
