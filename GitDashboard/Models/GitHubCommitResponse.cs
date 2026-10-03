namespace GitDashboard.Models;

public class GitHubCommitResponse
{
    public string Sha { get; set; } = string.Empty;
    public GitHubCommit Commit { get; set; } = new();
}

public class GitHubCommit
{
    public GitHubCommitAuthor Author { get; set; } = new();
}

public class GitHubCommitAuthor
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}