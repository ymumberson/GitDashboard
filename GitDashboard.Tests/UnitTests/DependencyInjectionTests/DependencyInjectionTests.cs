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
}