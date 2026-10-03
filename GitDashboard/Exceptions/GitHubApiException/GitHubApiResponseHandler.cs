using Microsoft.AspNetCore.Diagnostics;

namespace GitDashboard.Exceptions;

public class GitHubApiResponseHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not GitHubApiException)
        {
            return false;
        }

        await Results.Problem(
            statusCode: StatusCodes.Status502BadGateway,
            title: "GitHub API error",
            detail: exception.Message
        ).ExecuteAsync(httpContext);

        return true;
    }
}