using GitDashboard.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GitDashboard.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void RepositorySources_AreRegistered()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();

        using var scope = factory.Services.CreateScope();

        // Act
        var sources = scope.ServiceProvider.GetServices<IRepositorySource>().ToList();

        // Assert
        Assert.Contains(sources, source => source is LocalRepositorySource);
        Assert.Contains(sources, source => source is GitHubRepositorySource);
    }

    [Fact]
    public void GitHubHttpClient_IsConfiguredCorrectly()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();

        using var scope = factory.Services.CreateScope();

        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

        // Act
        var client = httpClientFactory.CreateClient(nameof(GitHubRepositorySource));

        // Assert
        Assert.Equal(new Uri("https://api.github.com"), client.BaseAddress);

        Assert.Contains(
            client.DefaultRequestHeaders.UserAgent,
            product => product.Product?.Name == "GitDashboard"
        );

        Assert.Contains(
            client.DefaultRequestHeaders.Accept,
            mediaType => mediaType.MediaType == "application/vnd.github+json"
        );
    }
}