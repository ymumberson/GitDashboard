using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeStatisticsService : IStatisticsService
{
    private readonly RepositoryStats _stats;

    public FakeStatisticsService(RepositoryStats stats)
    {
        _stats = stats;
    }

    public RepositoryStats Calculate(List<Commit> commits)
    {
        return _stats;
    }
}