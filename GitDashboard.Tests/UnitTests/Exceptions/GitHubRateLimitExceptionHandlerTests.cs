using GitDashboard.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class GitHubRateLimitExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WithResetTime_Returns429AndRetryAfter()
    {
        // Arrange
        var resetAt = DateTimeOffset.UtcNow.AddMinutes(5);

        var exception = new GitHubRateLimitException(
            "GitHub API rate limit reached.",
            resetAt);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();

        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        context.Response.Body = new MemoryStream();

        var handler = new GitHubRateLimitExceptionHandler();

        // Act
        var handled = await handler.TryHandleAsync(
            context,
            exception,
            CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(429, context.Response.StatusCode);

        Assert.True(
            context.Response.Headers.ContainsKey("Retry-After"));

        var retryAfter = int.Parse(
            context.Response.Headers.RetryAfter.ToString());

        Assert.InRange(retryAfter, 1, 300);
    }
}