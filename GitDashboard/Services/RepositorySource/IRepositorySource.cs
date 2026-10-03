using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IRepositorySource
{
    Task<List<Commit>> GetCommitsAsync(RepositoryReference repository, CancellationToken cancellationToken);

    bool CanHandle(RepositoryReference  repository);
}