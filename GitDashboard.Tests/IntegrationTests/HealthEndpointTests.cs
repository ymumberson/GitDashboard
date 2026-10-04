using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GitDashboard.Tests;

public class HealthEndpointTests
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}