using System.ComponentModel.DataAnnotations;
using GitDashboard.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GitDashboard.Tests;

public class GitHubAppOptionsTests
{
    [Fact]
    public void GitHubAppOptions_WithMissingClientId_IsInvalid()
    {
        var services = new ServiceCollection();

        services
            .AddOptions<GitHubAppOptions>()
            .Configure(options =>
            {
                options.ClientSecret = "test-secret";
                options.CallbackUrl = "https://example.com/callback";
            })
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.ClientId) &&
                    !string.IsNullOrWhiteSpace(options.ClientSecret) &&
                    Uri.TryCreate(
                        options.CallbackUrl,
                        UriKind.Absolute,
                        out var callbackUri) &&
                    (callbackUri.Scheme == Uri.UriSchemeHttps ||
                    callbackUri.IsLoopback))
            .ValidateOnStart();

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<GitHubAppOptions>>();

        Assert.Throws<OptionsValidationException>(() => options.Value);
    }
}