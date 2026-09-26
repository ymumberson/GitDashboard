using System.Net;
using System.Net.Http.Json;
using GitDashboard.Models;
using GitDashboard.Services;
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
                    services.RemoveAll<IGitService>();

                    services.AddSingleton<IGitService>(
                        new FakeGitService(
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
                    services.RemoveAll<IGitService>();

                    services.AddSingleton<IGitService>(
                        new FakeGitServiceThatThrows()
                    );
                });
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/analyse?path=/invalid/repository");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}