using System.Diagnostics;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitService : IGitService
{
    private readonly IGitRunner _gitRunner;

    public GitService(IGitRunner gitRunner)
    {
        _gitRunner = gitRunner;
    }

    public async Task<List<Commit>> GetCommitsAsync(string repositoryPath)
    {
        var output = await _gitRunner.GetLogAsync(repositoryPath);

        return output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseCommit)
            .ToList();
    }

    private static Commit ParseCommit(string line)
    {
        var parts = line.Split('|');

        if (parts.Length != 3)
        {
            throw new FormatException($"Invalid Git commit format: {line}");
        }

        return new Commit(
            Hash: parts[0],
            Author: parts[1],
            Date: DateOnly.Parse(parts[2])
        );
    }
}