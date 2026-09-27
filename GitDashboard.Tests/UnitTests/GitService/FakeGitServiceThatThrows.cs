using GitDashboard.Exceptions;
using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class FakeGitServiceThatThrows : IGitService
{
    public Task<List<Commit>> GetCommitsAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        throw new InvalidRepositoryException("The repository could not be accessed.");
    }
}