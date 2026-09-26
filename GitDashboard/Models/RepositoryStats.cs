namespace GitDashboard.Models;

public record RepositoryStats(
    int TotalCommits,
    Dictionary<string, int> CommitsByAuthor,
    Dictionary<DateOnly, int> CommitsByDate
);