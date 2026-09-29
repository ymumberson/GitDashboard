using System.Net;
using System.Net.Http.Json;
using GitDashboard.Exceptions;
using GitDashboard.Models;
using GitDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GitDashboard.Tests.Api;

public class AnalyseEndpointTests
{
    [Fact]
    public async Task Analyse_ReturnsRepositoryStatistics()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IRepositorySource>();

                    services.AddSingleton<IRepositorySource>(
                        new FakeRepositorySource(
                            new List<Commit>
                            {
                                new(
                                    "abc123",
                                    "Alice",
                                    new DateOnly(2026, 9, 21)
                                ),
                                new(
                                    "def456",
                                    "Bob",
                                    new DateOnly(2026, 9, 22)
                                )
                            }
                        )
                    );
                });
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/analyse?path=/test/repository");

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<RepositoryStats>();

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCommits);
        Assert.Equal(1, result.CommitsByAuthor["Alice"]);
        Assert.Equal(1, result.CommitsByAuthor["Bob"]);
    }

    [Fact]
    public async Task Analyse_WithInvalidRepository_ReturnsBadRequest()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IRepositorySource>();

                    services.AddSingleton<IRepositorySource>(
                        new FakeRepositorySource(
                            new InvalidRepositoryException(
                                "The repository could not be accessed."
                            )
                        )
                    );
                });
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/analyse?path=/invalid/repository");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Equal("Invalid repository", problem.Title);
    }

    [Fact]
    public async Task Analyse_WithoutPath_ReturnsBadRequest()
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/analyse");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Analyse_WithEmptyPath_ReturnsBadRequest() {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/analyse?path=");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}