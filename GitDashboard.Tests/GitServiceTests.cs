using GitDashboard.Services;

namespace GitDashboard.Tests;

public class GitServiceTests
{
    [Fact]
    public async Task GetCommitsAsync_ParseGitLog()
    {
        var gitRunner = new FakeGitRunner(
            "abc123|Alice|2026-09-25\n" +
            "def456|Bob|2026-09-24"
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
        var gitRunner = new FakeGitRunner("");

        var gitService = new GitService(gitRunner);

        var commits = await gitService.GetCommitsAsync("/fake/repository");

        Assert.Empty(commits);
    }

    [Fact]
    public async Task GetCommitsAsync_WithMalformedGitOutput_ThrowsFormatException()
    {
        var gitRunner = new FakeGitRunner("abc123|Alice");

        var gitService = new GitService(gitRunner);

        await Assert.ThrowsAsync<FormatException>(
            () => gitService.GetCommitsAsync("/fake/repository")
        );
    }
}
