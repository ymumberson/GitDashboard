namespace GitDashboard.Tests;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public HttpRequestMessage? ReceivedRequest {get; private set;} 

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ReceivedRequest = request;
        
        return Task.FromResult(_response);
    }
}