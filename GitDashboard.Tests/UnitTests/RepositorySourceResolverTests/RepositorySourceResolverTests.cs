using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class RepositorySourceResolverTests
{
    [Fact]
    public void Resolve_WithLocalRepository_ReturnsLocalSource()
    {
        var localSource = new FakeRepositorySource(typeof(LocalRepositoryReference), []);
        var githubSource = new FakeRepositorySource(typeof(GitHubRepositoryReference), []);

        var resolver = new RepositorySourceResolver(
            new[] { localSource, githubSource }
        );

        var repository = new LocalRepositoryReference("path/to/local/repo");

        var result = resolver.Resolve(repository);

        Assert.Same(localSource, result);
    }

    [Fact]
    public void Resolve_WithGitHubRepository_ReturnsGitHubSource()
    {
        var localSource = new FakeRepositorySource(typeof(LocalRepositoryReference), []);
        var githubSource = new FakeRepositorySource(typeof(GitHubRepositoryReference), []);

        var resolver = new RepositorySourceResolver(
            new[] { localSource, githubSource }
        );

        var repository = new GitHubRepositoryReference("Owner Name", "Repo Name");

        var result = resolver.Resolve(repository);

        Assert.Same(githubSource, result);
    }

    [Fact]
    public void Resolve_WhenNoSourceCanHandleRepository_ThrowsInvalidOperationException()
    {
        var localSource = new FakeRepositorySource(typeof(LocalRepositoryReference),[]);
        var resolver = new RepositorySourceResolver(new[] {localSource});

        var repository = new GitHubRepositoryReference("Owner Name", "Repo Name");

        Assert.Throws<InvalidOperationException>(
            () => resolver.Resolve(repository)
        );
    }
}