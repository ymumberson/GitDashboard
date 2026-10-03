using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeRepositorySource : IRepositorySource
{
    private readonly List<Commit>? _commits;
    private readonly Exception? _exception;
    private readonly Type _repositoryType;

    public FakeRepositorySource(Type repositoryType, List<Commit>? commits = null, Exception? exception = null)
    {
        _repositoryType = repositoryType;
        _commits = commits;
        _exception = exception;
    }

    public FakeRepositorySource(Type repositoryType, List<Commit> commits )
    {
        _repositoryType = repositoryType;
        _commits = commits;
    }

    public FakeRepositorySource(Type repositoryType, Exception exception)
    {
        _repositoryType = repositoryType;
        _exception = exception;
    }

    public bool CanHandle(RepositoryReference repository)
    {
        return repository.GetType() == _repositoryType;
    }

    public Task<List<Commit>> GetCommitsAsync(RepositoryReference repository, CancellationToken cancellationToken)
    {
        if (_exception is not null)
        {
            throw _exception;
        }
        
        return Task.FromResult(_commits!);
    }
}