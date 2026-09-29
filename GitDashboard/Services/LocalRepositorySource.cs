using GitDashboard.Models;
using GitDashboard.Services;

public class LocalRepositorySource : IRepositorySource
{
    private readonly IGitService _gitService;

    public LocalRepositorySource(IGitService gitService)
    {
        _gitService = gitService;
    }
    
    public Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        return _gitService.GetCommitsAsync(repositoryPath, cancellationToken);
    }
}