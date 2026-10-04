using System.Net;
using System.Net.Http.Headers;
using GitDashboard.Exceptions;
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

    [Fact]
    public async Task GetCommitsAsync_RequestsCorrectRepository()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "[]",
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

        var repository = new GitHubRepositoryReference("username", "repository");

        // Act
        await source.GetCommitsAsync(repository, CancellationToken.None);

        // Assert
        Assert.NotNull(handler.ReceivedRequest);

        Assert.Equal("/repos/username/repository/commits", handler.ReceivedRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetCommitsAsync_WithMissingRepository_ReturnsInvalidRepositoryException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        var source = new GitHubRepositorySource(httpClient);

        var repository = new GitHubRepositoryReference("username", "repository");

        // Act & assert
        await Assert.ThrowsAsync<InvalidRepositoryException>(
            () => source.GetCommitsAsync(repository, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetCommitsAsync_PassesCancellationTokenToHttpClient()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "[]",
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        var handler = new FakeHttpMessageHandler(response, true);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        var source = new GitHubRepositorySource(httpClient);

        var repository = new GitHubRepositoryReference("username", "repository");

        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        var task = source.GetCommitsAsync(repository, cancellationTokenSource.Token);

        cancellationTokenSource.Cancel();

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => task
        );
    }

    [Fact]
    public async Task GetCommitsAsync_WhenGitHubReturnsForbidden_ThrowsHttpRequestException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        var source = new GitHubRepositorySource(httpClient);

        var repository = new GitHubRepositoryReference("username", "repository");

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => source.GetCommitsAsync(repository, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetCommitsAsync_WithToken_SendsAuthorizationHeader()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "[]",
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "test-github-token");

        var source = new GitHubRepositorySource(httpClient);

        var repository = new GitHubRepositoryReference("username", "repository");
        
        // Act
        await source.GetCommitsAsync(repository, CancellationToken.None);

        // Assert
        Assert.NotNull(handler.ReceivedRequest);

        Assert.Equal(
        "Bearer",
        handler.ReceivedRequest.Headers.Authorization?.Scheme);

        Assert.Equal(
            "test-github-token",
            handler.ReceivedRequest.Headers.Authorization?.Parameter);
    }
}