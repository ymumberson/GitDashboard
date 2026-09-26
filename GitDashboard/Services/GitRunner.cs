using System.Diagnostics;
using GitDashboard.Exceptions;

namespace GitDashboard.Services;

public class GitRunner : IGitRunner
{
    public async Task<string> GetLogAsync(string repositoryPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "log --pretty=format:\"%H|%an|%ad\" --date=short",
                WorkingDirectory = repositoryPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidRepositoryException($"Unable to read repository: {error}");
        }

        return output;
    }
}