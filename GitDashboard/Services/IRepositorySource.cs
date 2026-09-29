using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IRepositorySource
{
    Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken);
}