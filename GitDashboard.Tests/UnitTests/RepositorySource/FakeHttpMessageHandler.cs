namespace GitDashboard.Tests;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;
    public List<HttpRequestMessage> ReceivedRequests {get;} = [];
    public CancellationToken? ReceivedCancellationToken {get; private set;}
    private readonly bool _waitForCancellation;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory, bool waitForCancellation = false)
    {
        _responseFactory = responseFactory;
        _waitForCancellation = waitForCancellation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ReceivedRequests.Add(request);
        ReceivedCancellationToken = cancellationToken;

        if (_waitForCancellation)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        
        return _responseFactory(request);
    }
}