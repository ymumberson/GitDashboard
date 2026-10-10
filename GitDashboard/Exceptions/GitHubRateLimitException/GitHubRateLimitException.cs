namespace GitDashboard.Exceptions;

public class GitHubRateLimitException : Exception
{
    public DateTimeOffset? ResetsAt {get;}

    public GitHubRateLimitException(string message, DateTimeOffset? resetsAt = null) : base(message) {ResetsAt = resetsAt;}
}