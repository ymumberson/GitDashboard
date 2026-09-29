using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class RepositorySourceTests
{
    [Fact]
    public async Task GetCommitsAsync_PassesRepositoryPath()
    {
        var gitService = new FakeGitService([]);

        var repositorySource = new LocalRepositorySource(gitService);

        await repositorySource.GetCommitsAsync("/path/to/repository", CancellationToken.None);

        Assert.Equal("/path/to/repository", gitService.PassedRepositoryPath);
    }

    [Fact]
    public async Task GetCommitsAsync_PassesCancellationToken()
    {
        var gitService = new FakeGitService([]);

        var repositorySource = new LocalRepositorySource(gitService);
        var cancellationTokenSource = new CancellationTokenSource();

        await repositorySource.GetCommitsAsync("/path/to/repository", cancellationTokenSource.Token);

        Assert.Equal(cancellationTokenSource.Token, gitService.PassedCancellationToken);
    }

    [Fact]
    public async Task GetCommitsAsync_ReturnsCommitsFromGitService()
    {
        var commits = new List<Commit>
        {
             new(
                "abc123",
                "Alice",
                new DateOnly(2026, 9, 21)
            ),
            new(
                "def456",
                "Bob",
                new DateOnly(2026, 9, 22)
            )
        };
        
        var gitService = new FakeGitService(commits);

        var repositorySource = new LocalRepositorySource(gitService);

        var result = await repositorySource.GetCommitsAsync("/path/to/repository", CancellationToken.None);

        Assert.Equal(commits, result);
    }
}