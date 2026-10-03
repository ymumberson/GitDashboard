namespace GitDashboard.Tests;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public HttpRequestMessage? ReceivedRequest {get; private set;}
    public CancellationToken? ReceivedCancellationToken {get; private set;}
    private readonly bool _waitForCancellation;

    public FakeHttpMessageHandler(HttpResponseMessage response, bool waitForCancellation = false)
    {
        _response = response;
        _waitForCancellation = waitForCancellation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ReceivedRequest = request;
        ReceivedCancellationToken = cancellationToken;

        if (_waitForCancellation)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        
        return _response;
    }
}