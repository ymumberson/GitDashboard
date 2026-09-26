using System.Diagnostics;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitService
{
    public async Task<List<Commit>> GetCommitsAsync(string repositoryPath)
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
            throw new Exception($"Git failed: {error}");
        }

        return output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseCommit)
            .ToList();
    }

    private static Commit ParseCommit(string line)
    {
        var parts = line.Split('|');
        return new Commit(
            Hash: parts[0],
            Author: parts[1],
            Date: DateOnly.Parse(parts[2])
        );
    }
}