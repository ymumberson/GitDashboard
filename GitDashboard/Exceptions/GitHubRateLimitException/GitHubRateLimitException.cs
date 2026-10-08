namespace GitDashboard.Exceptions;

public class GitHubRateLimitException : Exception
{
    public GitHubRateLimitException(string message) : base(message) {}
}