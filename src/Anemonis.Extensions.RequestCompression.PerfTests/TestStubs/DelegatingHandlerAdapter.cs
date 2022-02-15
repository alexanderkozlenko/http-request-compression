namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class DelegatingHandlerAdapter : DelegatingHandler
{
    public DelegatingHandlerAdapter(DelegatingHandler targetHandler, Func<HttpRequestMessage, Task<HttpResponseMessage>> primaryHandler)
        : base(targetHandler)
    {
        targetHandler.InnerHandler = new DelegatingPrimaryHandler(primaryHandler);
    }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.SendAsync(request, cancellationToken);
    }
}
