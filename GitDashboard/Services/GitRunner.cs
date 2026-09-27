using System.Diagnostics;
using GitDashboard.Exceptions;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitRunner : IGitRunner
{
    public async Task<GitCommandResult> RunAsync(string repositoryPath, string arguments, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(repositoryPath))
        {
            throw new InvalidRepositoryException($"Repository does not exist: {repositoryPath}");
        }
        
        var process = new Process
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

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        return new GitCommandResult(
            Output: output,
            Error: error,
            ExitCode: process.ExitCode
        );
    }
}