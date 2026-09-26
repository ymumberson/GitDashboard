namespace GitDashboard.Models;

public record GitCommandResult(
    string Output,
    string Error,
    int ExitCode
);