using GitDashboard.Services;
using GitDashboard.Models;
using GitDashboard.Exceptions;

namespace GitDashboard.Tests;

public class GitServiceTests
{
    [Fact]
    public async Task GetCommitsAsync_ParseGitLog()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult (
                "abc123|Alice|2026-09-25\ndef456|Bob|2026-09-24",
                "",
                0
            )
        );

        var gitService = new GitService(gitRunner);

        var commits = await gitService.GetCommitsAsync("/fake/repository");

        Assert.Equal(2, commits.Count);

        Assert.Equal("abc123", commits[0].Hash);
        Assert.Equal("Alice", commits[0].Author);
        Assert.Equal(
            new DateOnly(2026, 9, 25),
            commits[0].Date
        );
    }

    [Fact]
    public async Task GetCommitsAsync_WithNoCommits_ReturnsEmptyList()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult (
                "",
                "",
                0
            )
        );

        var gitService = new GitService(gitRunner);

        var commits = await gitService.GetCommitsAsync("/fake/repository");

        Assert.Empty(commits);
    }

    [Fact]
    public async Task GetCommitsAsync_WithMalformedGitOutput_ThrowsFormatException()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult (
                "abc123|Alice",
                "",
                0
            )
        );

        var gitService = new GitService(gitRunner);

        await Assert.ThrowsAsync<FormatException>(
            () => gitService.GetCommitsAsync("/fake/repository")
        );
    }

    [Fact]
    public async Task GetCommitsAsync_PassesRepositoryPathToGitRunner()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult (
                "",
                "",
                0
            )
        );
        var gitService = new GitService(gitRunner);

        await gitService.GetCommitsAsync("/my/test/repository");

        Assert.Equal("/my/test/repository", gitRunner.ReceivedRepositoryPath);
    }

    [Fact]
    public async Task GetCommitsAsync_PassesArgumentsPathToGitRunner()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult("", "", 0)
        );

        var gitService = new GitService(gitRunner);

        await gitService.GetCommitsAsync("/path/to/repository");

        Assert.Equal("log --pretty=format:\"%H|%an|%ad\" --date=short", gitRunner.RecievedArguments);
    }

    [Fact]
    public async Task GetCommitsAsync_WithInvalidRepository_ThrowsInvalidRepositoryException()
    {
        // Arrange
        var gitRunner = new FakeGitRunner(
            new GitCommandResult(
                "",
                "fatal: not a git repository",
                128
            )
        );
        var service = new GitService(gitRunner);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRepositoryException>(
            () => service.GetCommitsAsync("C:\\this\\does\\not\\exist")
        );
    }
}
