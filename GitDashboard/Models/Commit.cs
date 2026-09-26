namespace GitDashboard.Models;

public record Commit(
    string Hash,
    string Author,
    DateOnly Date
);