using System.Net;
using GitDashboard.Exceptions;
using GitDashboard.Models;

namespace GitDashboard.Services;

public class GitHubRepositorySource : IRepositorySource
{
    private readonly HttpClient _httpClient;

    public GitHubRepositorySource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public bool CanHandle(RepositoryReference repository)
    {
        return repository is GitHubRepositoryReference;
    }

    public async Task<List<Commit>> GetCommitsAsync(RepositoryReference repository, CancellationToken cancellationToken)
    {
        if (repository is not GitHubRepositoryReference gitHubRepositoryReference)
        {
            throw new ArgumentException(
                "Repository must be a GitHub repository.",
                nameof(repository));
        }

        var response = await _httpClient.GetAsync(
            $"/repos/{gitHubRepositoryReference.Owner}/{gitHubRepositoryReference.Name}/commits",
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidRepositoryException($"The GitHub repository could not be found.");
        }

        response.EnsureSuccessStatusCode();

        var commits = await response.Content.ReadFromJsonAsync<List<GitHubCommitResponse>>(cancellationToken);

        return commits?
            .Select(commit => new Commit(commit.Sha, commit.Commit.Author.Name, DateOnly.FromDateTime(commit.Commit.Author.Date)))
            .ToList()
            ?? [];
    }
}