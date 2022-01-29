namespace Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;

internal sealed class TestDelegatingHandler : DelegatingHandler
{
    public TestDelegatingHandler(DelegatingHandler targetHandler, Func<HttpRequestMessage, Task<HttpResponseMessage>> bedrockHandler)
        : base(targetHandler)
    {
        targetHandler.InnerHandler = new TestPrimaryHandler(bedrockHandler);
    }

    public new HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.Send(request, cancellationToken);
    }

    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.SendAsync(request, cancellationToken);
    }
}
