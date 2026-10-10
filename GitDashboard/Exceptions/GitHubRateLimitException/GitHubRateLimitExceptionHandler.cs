using System.Globalization;
using Microsoft.AspNetCore.Diagnostics;

namespace GitDashboard.Exceptions;

public class GitHubRateLimitExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not GitHubRateLimitException rateLimitException)
        {
            return false;
        }

        if (rateLimitException.ResetsAt is {} resetsAt)
        {
            var retryAfterSeconds = Math.Max(
                0,
                (int)Math.Ceiling((resetsAt - DateTimeOffset.UtcNow).TotalSeconds)
            );

            httpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
        }

        await Results.Problem(
            statusCode: StatusCodes.Status429TooManyRequests,
            title: "GitHub API rate limit reached",
            detail: exception.Message
        ).ExecuteAsync(httpContext);

        return true;
    }
}