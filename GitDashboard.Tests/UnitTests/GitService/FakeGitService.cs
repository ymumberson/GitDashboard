using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeGitService : IGitService
{
    private readonly List<Commit> _commits;
    public string? PassedRepositoryPath {get; private set;}
    public CancellationToken? PassedCancellationToken {get; private set;}

    public FakeGitService(List<Commit> commits)
    {
        _commits = commits;
    }

    public Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        PassedRepositoryPath = repositoryPath;
        PassedCancellationToken = cancellationToken;

        return Task.FromResult(_commits);
    }
}