using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IGitRunner
{
    Task<GitCommandResult> RunAsync(string repositoryPath, string arguments);
}