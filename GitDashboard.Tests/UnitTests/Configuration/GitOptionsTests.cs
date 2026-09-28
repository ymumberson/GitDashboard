using System.ComponentModel.DataAnnotations;
using GitDashboard.Configuration;

namespace GitDashboard.Tests;

public class GitOptionsTests
{
    [Fact]
    public void GitOptions_WithEmptyLogArguments_IsInvalid()
    {
        var options = new GitOptions
        {
            LogArguments = ""
        };

        var validationContext = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            options,
            validationContext,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
    }
}