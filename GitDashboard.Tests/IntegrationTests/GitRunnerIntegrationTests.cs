using System.Diagnostics;
using GitDashboard.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace GitDashboard.Tests;

public class GitRunnerIntegrationTests
{
    [Fact]
    public async Task RunAsync_WithGitRepository_ReturnsCommitLog()
    {
        var repositoryPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(repositoryPath);

        try
        {
            await RunGitCommand(repositoryPath, "init");

            await File.WriteAllTextAsync(
                Path.Combine(repositoryPath, "test.txt"),
                "Hello, World!"
            );

            await RunGitCommand(repositoryPath, "add .");

            await RunGitCommand(
                repositoryPath,
                "commit -m \"Initial commit\" --author=\"Alice <alice@example.com>\""
            );

            var logger = NullLogger<GitRunner>.Instance;
            var gitRunner = new GitRunner(logger);

            var result = await gitRunner.RunAsync(
                repositoryPath,
                "log --pretty=format:\"%H|%an|%ad\" --date=short",
                new CancellationToken()
            );

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("|Alice|", result.Output);
        }
        finally
        {
            DeleteDirectory(repositoryPath);
        }
    }

    private static async Task RunGitCommand(string repositoryPath, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = repositoryPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo)!;

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        Assert.True(
            process.ExitCode == 0,
            $"Git command failed: git {arguments}\n" +
            $"Exit code: {process.ExitCode}\n" +
            $"Standard error: {error}\n" +
            $"Standard output: {output}");
    }

    private static void DeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        var directoryInfo = new DirectoryInfo(path);

        foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            file.Attributes = FileAttributes.Normal;
        }

        foreach (var directory in directoryInfo.GetDirectories("*", SearchOption.AllDirectories))
        {
            directory.Attributes = FileAttributes.Normal;
        }

        directoryInfo.Attributes = FileAttributes.Normal;

        Directory.Delete(path, true);
    }
}