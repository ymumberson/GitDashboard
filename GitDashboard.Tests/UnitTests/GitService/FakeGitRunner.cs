using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeGitRunner : IGitRunner
{
    private readonly GitCommandResult _commandResult;
    public string? ReceivedRepositoryPath {get; private set;}
    public string? RecievedArguments {get; private set;}

    public FakeGitRunner(GitCommandResult commandResult)
    {
        _commandResult = commandResult;
    }

    public Task<GitCommandResult> RunAsync(string repositoryPath, string arguments)
    {
        ReceivedRepositoryPath = repositoryPath;
        RecievedArguments = arguments;

        return Task.FromResult(_commandResult);
    }
}