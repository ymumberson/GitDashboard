using GitDashboard.Models;

namespace GitDashboard.Services;

public class StatisticsService : IStatisticsService
{
    public RepositoryStats Calculate(List<Commit> commits)
    {    
        var commitsByAuthor = commits
            .GroupBy(commit => commit.Author)
            .ToDictionary(group => group.Key, group => group.Count());

        var commitsByDate = commits
            .GroupBy(commit => commit.Date)
            .ToDictionary(group => group.Key, group => group.Count());

        return new RepositoryStats(
            TotalCommits: commits.Count,
            CommitsByAuthor: commitsByAuthor,
            CommitsByDate: commitsByDate
        );
    }
}