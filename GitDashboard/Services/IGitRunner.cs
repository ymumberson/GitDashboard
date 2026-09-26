namespace GitDashboard.Services;

public interface IGitRunner
{
    Task<string> GetLogAsync(string repositoryPath);
}