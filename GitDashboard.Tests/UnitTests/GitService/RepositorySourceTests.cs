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
}