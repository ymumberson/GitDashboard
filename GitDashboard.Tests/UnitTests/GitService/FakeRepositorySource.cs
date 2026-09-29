using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeRepositorySource : IRepositorySource
{
    private readonly List<Commit>? _commits;
    private readonly Exception? _exception;

    public FakeRepositorySource(List<Commit> commits)
    {
        _commits = commits;
    }

    public FakeRepositorySource(Exception exception)
    {
        _exception = exception;
    }
    
    public Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        if (_exception is not null)
        {
            throw _exception;
        }
        
        return Task.FromResult(_commits!);
    }
}