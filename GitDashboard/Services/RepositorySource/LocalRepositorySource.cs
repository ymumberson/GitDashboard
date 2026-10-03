using GitDashboard.Models;
namespace GitDashboard.Services;

public class LocalRepositorySource : IRepositorySource
{
    private readonly IGitService _gitService;

    public LocalRepositorySource(IGitService gitService)
    {
        _gitService = gitService;
    }
    
    public Task<List<Commit>> GetCommitsAsync(RepositoryReference repository, CancellationToken cancellationToken)
    {
        var localRepository = (LocalRepositoryReference)repository;
        
        return _gitService.GetCommitsAsync(localRepository.Path, cancellationToken);
    }

    public bool CanHandle(RepositoryReference repository)
    {
        return repository is LocalRepositoryReference;
    }
}