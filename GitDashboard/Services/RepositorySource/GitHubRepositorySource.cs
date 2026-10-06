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

        const int pageSize = 100;
        var page = 1;
        var commits = new List<GitHubCommitResponse>();

        while (true)
        {
            var response = await _httpClient.GetAsync(
                $"/repos/{gitHubRepositoryReference.Owner}/{gitHubRepositoryReference.Name}/commits" +
                $"?per_page={pageSize}&page={page}",
                cancellationToken
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidRepositoryException($"The GitHub repository could not be found.");
            }

            response.EnsureSuccessStatusCode();

            var pageCommits = await response.Content.ReadFromJsonAsync<List<GitHubCommitResponse>>(cancellationToken) ?? [];
        
            commits.AddRange(pageCommits);

            if (pageCommits.Count < pageSize)
            {
                break;
            }

            page++;
        }
        
        return commits?
            .Select(commit => new Commit(commit.Sha, commit.Commit.Author.Name, DateOnly.FromDateTime(commit.Commit.Author.Date)))
            .ToList()
            ?? [];
    }
}