using GitDashboard.Exceptions;
using GitDashboard.Services;
using GitDashboard.Configuration;
using GitDashboard.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using GitDashboard.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using GitDashboard.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddOptions<GitOptions>()
   .Bind(builder.Configuration.GetSection("Git"))
   .ValidateDataAnnotations()
   .ValidateOnStart();

builder.Services
    .AddOptions<GitHubOptions>()
    .Bind(builder.Configuration.GetSection("GitHub"));
builder.Services.AddOptions<GitHubAppOptions>()
   .Bind(builder.Configuration.GetSection("GitHubApp"))
   .Validate(options =>
      !string.IsNullOrWhiteSpace(options.ClientId) &&
      !string.IsNullOrWhiteSpace(options.ClientSecret) &&
      Uri.TryCreate(
                options.CallbackUrl,
                UriKind.Absolute,
                out var callbackUri) &&
            (callbackUri.Scheme == Uri.UriSchemeHttps ||
             callbackUri.IsLoopback),
      "GitHub App configuration is incomplete.")
   .ValidateOnStart();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<InvalidRepositoryExceptionHandler>();
builder.Services.AddExceptionHandler<GitHubApiExceptionHandler>();
builder.Services.AddExceptionHandler<GitHubRateLimitExceptionHandler>();

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IGitRunner, GitRunner>();
builder.Services.AddSingleton<IGitService, GitService>();
builder.Services.AddSingleton<IRepositorySource, LocalRepositorySource>();
builder.Services.AddHttpClient<GitHubRepositorySource>((serviceProvider, client) =>
{
   var options = serviceProvider
        .GetRequiredService<IOptions<GitHubOptions>>()
        .Value;
   
   client.BaseAddress = new Uri("https://api.github.com");

   client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(
            "application/vnd.github+json"));

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "GitDashboard");

   if (!string.IsNullOrWhiteSpace(options.Token))
   {
      client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                options.Token);
   }
});
builder.Services.AddSingleton<IRepositorySource>(sp =>
    sp.GetRequiredService<GitHubRepositorySource>());
builder.Services.AddSingleton<IRepositorySourceResolver, RepositorySourceResolver>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
builder.Services.AddSingleton<RepositoryAnalysisService>();

builder.Services.AddSingleton<GitHubUserTokenStore>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
      options.Cookie.Name = "GitDashboard.Auth";
      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      options.Cookie.SameSite = SameSiteMode.Lax;
      options.ExpireTimeSpan = TimeSpan.FromHours(8);
      options.SlidingExpiration = true;
   });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("GitHubOAuth", client =>
{
   client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json")
   );

   client.DefaultRequestHeaders.UserAgent.ParseAdd("GitDashboard");
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGet("api/analyse", async (
   string? path,
   string? owner,
   string? name,
   RepositoryAnalysisService service,
   CancellationToken cancellationToken) =>
{
   RepositoryReference repository;
   
   if (!string.IsNullOrWhiteSpace(path))
   {
      repository = new LocalRepositoryReference(path);
   }
   else if (!string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(name))
   {
      repository = new GitHubRepositoryReference(owner, name);
   }
   else
   {
      return Results.Problem(
         statusCode: StatusCodes.Status400BadRequest,
         title: "Invalid request",
         detail: "The repository path or GitHub repository is required"
      );
   }
   
   return Results.Ok(await service.AnalyseAsync(repository, cancellationToken));
});

app.MapGitHubAuthEndpoints();

app.Run();
