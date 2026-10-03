namespace GitDashboard.Models;

public abstract record RepositoryReference {}

public record LocalRepositoryReference(string Path) : RepositoryReference;

public record GitHubRepositoryReference(string Owner, string Name) : RepositoryReference;