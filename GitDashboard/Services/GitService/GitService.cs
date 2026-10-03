using System.Diagnostics;
using GitDashboard.Configuration;
using GitDashboard.Exceptions;
using GitDashboard.Models;
using Microsoft.Extensions.Options;

namespace GitDashboard.Services;

public class GitService : IGitService
{
    private readonly IGitRunner _gitRunner;
    private readonly GitOptions _gitOptions;

    public GitService(IGitRunner gitRunner, IOptions<GitOptions> gitOptions)
    {
        _gitRunner = gitRunner;
        _gitOptions = gitOptions.Value;
    }

    public async Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        var result = await _gitRunner.RunAsync(repositoryPath, _gitOptions.LogArguments, cancellationToken);

        if (result.ExitCode != 0)
        {
            throw new InvalidRepositoryException($"Invalid repository");
        }

        return result.Output
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