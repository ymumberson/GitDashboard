using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IStatisticsService
{
    RepositoryStats Calculate(List<Commit> commits);
}