using GitDashboard.Models;

namespace GitDashboard.Services;

public class RepositoryAnalysisService
{
    private readonly IRepositorySourceResolver _repositorySourceResolver;
    private readonly IStatisticsService _statisticsService;

    public RepositoryAnalysisService(
        IRepositorySourceResolver repositorySourceResolver,
        IStatisticsService statisticsService)
    {
        _repositorySourceResolver = repositorySourceResolver;
        _statisticsService = statisticsService;
    }

    public async Task<RepositoryStats> AnalyseAsync(RepositoryReference repository, CancellationToken cancellationToken)
    {
        var source = _repositorySourceResolver.Resolve(repository);
        var commits = await source.GetCommitsAsync(repository, cancellationToken);

        return _statisticsService.Calculate(commits);
    }
}