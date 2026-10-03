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
        var localRepositoryReference = new LocalRepositoryReference("/path/to/repository");

        await repositorySource.GetCommitsAsync(localRepositoryReference, CancellationToken.None);

        Assert.Equal("/path/to/repository", gitService.PassedRepositoryPath);
    }

    [Fact]
    public async Task GetCommitsAsync_PassesCancellationToken()
    {
        var gitService = new FakeGitService([]);

        var repositorySource = new LocalRepositorySource(gitService);
        var cancellationTokenSource = new CancellationTokenSource();
        var localRepositoryReference = new LocalRepositoryReference("/path/to/repository");

        await repositorySource.GetCommitsAsync(localRepositoryReference, cancellationTokenSource.Token);

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
        var localRepositoryReference = new LocalRepositoryReference("/path/to/repository");

        var result = await repositorySource.GetCommitsAsync(localRepositoryReference, CancellationToken.None);

        Assert.Equal(commits, result);
    }
}