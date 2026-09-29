using GitDashboard.Models;

namespace GitDashboard.Services;

public class RepositoryAnalysisService
{
    private readonly IRepositorySource _repositorySource;
    private readonly IStatisticsService _statisticsService;

    public RepositoryAnalysisService(
        IRepositorySource repositorySource,
        IStatisticsService statisticsService)
    {
        _repositorySource = repositorySource;
        _statisticsService = statisticsService;
    }

    public async Task<RepositoryStats> AnalyseAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        var commits = await _repositorySource.GetCommitsAsync(repositoryPath, cancellationToken);

        return _statisticsService.Calculate(commits);
    }
}