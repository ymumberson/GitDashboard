using System.ComponentModel.DataAnnotations;

namespace GitDashboard.Configuration;

public class GitOptions
{
    [Required]
    public string LogArguments {get; set;} = "log --pretty=format:\"%H|%an|%ad\" --date=short";
}