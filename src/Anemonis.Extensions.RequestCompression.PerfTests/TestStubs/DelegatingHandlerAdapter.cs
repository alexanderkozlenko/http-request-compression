namespace Anemonis.Extensions.RequestCompression.PerfTests.TestStubs;

internal sealed class DelegatingHandlerAdapter : DelegatingHandler
{
    public DelegatingHandlerAdapter(DelegatingHandler targetHandler, Func<HttpRequestMessage, HttpResponseMessage> primaryHandler)
        : base(targetHandler)
    {
        targetHandler.InnerHandler = new DelegatingPrimaryHandler(primaryHandler);
    }

    public HttpResponseMessage InvokeSend(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Send(request, cancellationToken);
    }
}
