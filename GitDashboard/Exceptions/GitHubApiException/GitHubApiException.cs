namespace GitDashboard.Exceptions;

public class GitHubApiException : Exception
{
    public GitHubApiException(string message) : base(message) {}
}