using System.Diagnostics;
using GitDashboard.Exceptions;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitRunner : IGitRunner
{
    private readonly ILogger<GitRunner> _logger;

    public GitRunner(ILogger<GitRunner> logger)
    {
        _logger = logger;
    }
    
    public async Task<GitCommandResult> RunAsync(string repositoryPath, string arguments, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(repositoryPath))
        {
            _logger.LogError(
                "Git command failed. Repository does not exist: {RepositoryPath}",
                repositoryPath
            );
            throw new InvalidRepositoryException($"Repository does not exist: {repositoryPath}");
        }
        
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = repositoryPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        _logger.LogInformation(
            "Running git command in repository {RepositoryPath}: {Arguments}",
            repositoryPath,
            arguments
        );

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            _logger.LogError(
                "Git command failed with exit code {ExitCode}: {Error}",
                process.ExitCode,
                error
            );
        }
        else
        {
            _logger.LogInformation(
                "Git command succeeded in repository {RepositoryPath}: {Arguments}",
                repositoryPath,
                arguments
            );
        }

        return new GitCommandResult(
            Output: output,
            Error: error,
            ExitCode: process.ExitCode
        );
    }
}