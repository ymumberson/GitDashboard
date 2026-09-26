using System.Diagnostics;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitService
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
        return new Commit(
            Hash: parts[0],
            Author: parts[1],
            Date: DateOnly.Parse(parts[2])
        );
    }
}