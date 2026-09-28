namespace GitDashboard.Configuration;

public class GitOptions
{
    public string LogArguments {get; set;} = "log --pretty=format:\"%H|%an|%ad\" --date=short";
}