using GitDashboard.Models;
using GitDashboard.Services;
using Microsoft.AspNetCore.Http;

namespace GitDashboard.Tests;

public class StatisticsServiceTests
{
    [Fact]
    public void Calculate_CountsTotalCommits()
    {
        var commits = new List<Commit>
        {
            new("abc123", "Alice", new DateOnly(2026, 9, 21)),
            new("def456", "Bob", new DateOnly(2026, 9, 22)),
            new("ghi789", "Alice", new DateOnly(2026, 9, 23))
        };

        var service = new StatisticsService();

        var result = service.Calculate(commits);

        Assert.Equal(3, result.TotalCommits);
    }

    [Fact]
    public void Calculate_CountsCommitsByAuthor()
    {
        var commits = new List<Commit>
        {
            new("abc123", "Alice", new DateOnly(2026, 9, 21)),
            new("def456", "Bob", new DateOnly(2026, 9, 22)),
            new("ghi789", "Alice", new DateOnly(2026, 9, 23))
        };

        var service = new StatisticsService();

        var result = service.Calculate(commits);

        Assert.Equal(2, result.CommitsByAuthor["Alice"]);
        Assert.Equal(1, result.CommitsByAuthor["Bob"]);
    }

    [Fact]
    public void Calculate_CountsCommitsByDate()
    {
        var commits = new List<Commit>
        {
            new("abc123", "Alice", new DateOnly(2026, 9, 21)),
            new("def456", "Bob", new DateOnly(2026, 9, 21)),
            new("ghi789", "Alice", new DateOnly(2026, 9, 22))
        };

        var service = new StatisticsService();

        var result = service.Calculate(commits);

        Assert.Equal(2, result.CommitsByDate[new DateOnly(2026, 9, 21)]);
        Assert.Equal(1, result.CommitsByDate[new DateOnly(2026, 9, 22)]);
    }
}