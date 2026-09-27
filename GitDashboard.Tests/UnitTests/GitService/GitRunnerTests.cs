using GitDashboard.Models;

namespace GitDashboard.Tests;

public class GitRunnerTests
{
    [Fact]
    public async Task RunAsync_WhenGitCommandFails_ReturnsNonZeroExitCode()
    {
        var gitRunner = new FakeGitRunner(
            new GitCommandResult(
                "",
                "fatal: not a git repository",
                128
            )
        );

        var result = await gitRunner.RunAsync("/Invalid/Directory", "", new CancellationToken());

        Assert.NotEqual(0, result.ExitCode);
    }
}