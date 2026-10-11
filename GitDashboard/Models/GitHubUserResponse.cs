using System.Text.Json.Serialization;

namespace GitDashboard.Models;

public sealed class GitHubUserResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("login")]
    public string? Login { get; set; }
}