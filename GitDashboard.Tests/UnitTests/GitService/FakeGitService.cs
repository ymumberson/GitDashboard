using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeGitService : IGitService
{
    private readonly List<Commit> _commits;

    public FakeGitService(List<Commit> commits)
    {
        _commits = commits;
    }

    public Task<List<Commit>> GetCommitsAsync(string repositoryPath)
    {
        return Task.FromResult(_commits);
    }
}