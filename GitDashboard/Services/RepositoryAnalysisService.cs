using GitDashboard.Models;

namespace GitDashboard.Services;

public class RepositoryAnalysisService
{
    private readonly IGitService _gitService;
    private readonly IStatisticsService _statisticsService;

    public RepositoryAnalysisService(
        IGitService gitService,
        IStatisticsService statisticsService)
    {
        _gitService = gitService;
        _statisticsService = statisticsService;
    }

    public async Task<RepositoryStats> AnalyseAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        var commits = await _gitService.GetCommitsAsync(repositoryPath, cancellationToken);

        return _statisticsService.Calculate(commits);
    }
}