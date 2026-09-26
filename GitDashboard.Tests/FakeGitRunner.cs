using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeGitRunner : IGitRunner
{
    private readonly string _output;
    public string? ReceivedRepositoryPath {get; private set;}

    public FakeGitRunner(string output)
    {
        _output = output;
    }

    public Task<string> GetLogAsync(string repositoryPath)
    {
        ReceivedRepositoryPath = repositoryPath;

        return Task.FromResult(_output);
    }
}