using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IGitService
{
    Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken);
}