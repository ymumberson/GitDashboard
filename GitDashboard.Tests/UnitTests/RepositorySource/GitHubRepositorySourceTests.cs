using System.Net;
using GitDashboard.Models;
using GitDashboard.Services;

namespace GitDashboard.Tests;

public class GitHubRepositorySourceTests
{
    [Fact]
    public async Task GetCommitsAsync_ReturnsCommitsFromGitHub()
    {
        // Arange
        var json = """
        [
          {
            "sha": "abc123",
            "commit": {
              "author": {
                "name": "Alice",
                "date": "2026-09-25T10:30:00Z"
              }
            }
          },
          {
            "sha": "def456",
            "commit": {
              "author": {
                "name": "Bob",
                "date": "2026-09-24T15:20:00Z"
              }
            }
          }
        ]
        """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                json,
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        var source = new GitHubRepositorySource(httpClient);

        var repository = new GitHubRepositoryReference("Owner Name", "Repo Name");

        // Act
        var commits = await source.GetCommitsAsync(repository, CancellationToken.None);

        // Assert
        Assert.Equal(2, commits.Count);

        Assert.Equal("abc123", commits[0].Hash);
        Assert.Equal("Alice", commits[0].Author);
        Assert.Equal(
            new DateOnly(2026, 9, 25),
            commits[0].Date);

        Assert.Equal("def456", commits[1].Hash);
        Assert.Equal("Bob", commits[1].Author);
        Assert.Equal(
            new DateOnly(2026, 9, 24),
            commits[1].Date);
    }
}