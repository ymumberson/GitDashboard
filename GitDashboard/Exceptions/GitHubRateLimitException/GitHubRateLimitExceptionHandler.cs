using Microsoft.AspNetCore.Diagnostics;

namespace GitDashboard.Exceptions;

public class GitHubRateLimitExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not GitHubRateLimitException)
        {
            return false;
        }

        await Results.Problem(
            statusCode: StatusCodes.Status429TooManyRequests,
            title: "GitHub API rate limit reached",
            detail: exception.Message
        ).ExecuteAsync(httpContext);

        return true;
    }
}