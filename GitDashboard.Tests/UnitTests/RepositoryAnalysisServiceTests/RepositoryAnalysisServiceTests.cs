using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class RepositoryAnalysisTests
{
    [Fact]
    public async Task AnalyseAsync_GetCommitsAndCalculateStatistics()
    {
        var commits = new List<Commit>
        {
            new("abc123", "Alice", new DateOnly(2026, 9, 21)),
            new("def456", "Bob", new DateOnly(2026, 9, 22))
        };

        var expectedStats = new RepositoryStats(
            TotalCommits: 2,
            CommitsByAuthor: new Dictionary<string, int>
            {
                ["Alice"] = 1,
                ["Bob"] = 1
            },
            CommitsByDate: new Dictionary<DateOnly, int>
            {
                [new DateOnly(2026, 9, 21)] = 1,
                [new DateOnly(2026, 9, 22)] = 1
            }
        );

        var repositorySource = new FakeRepositorySource(typeof(LocalRepositoryReference), commits);
        var statisticsService = new FakeStatisticsService(expectedStats);
        var repositoryReference = new LocalRepositoryReference("/test/repository");
        var repositoryResolver = new RepositorySourceResolver(new[] {repositorySource});

        var service = new RepositoryAnalysisService(repositoryResolver, statisticsService);

        var result = await service.AnalyseAsync(repositoryReference, CancellationToken.None);

        Assert.Same(expectedStats, result);
    }
}