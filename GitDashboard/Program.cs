using GitDashboard.Exceptions;
using GitDashboard.Services;
using GitDashboard.Configuration;
using GitDashboard.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

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

var app = builder.Build();

app.UseExceptionHandler();

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

app.MapGet("api/auth/github/login", (
   HttpContext context,
   IOptions<GitHubAppOptions> options
) =>
{
   var settings = options.Value;

   // This unique state value is used to prevent CSRF attacks.
   var state = WebEncoders.Base64UrlEncode(
      RandomNumberGenerator.GetBytes(32)
   );

   // This is the PKCE verifier and its S256 challenge.
   var codeVerifier = WebEncoders.Base64UrlEncode(
      RandomNumberGenerator.GetBytes(32)
   );
   var codeChallenge = WebEncoders.Base64UrlEncode(
      SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier))
   );

   var cookieOptions = new CookieOptions
   {
      HttpOnly = true,
      Secure = context.Request.IsHttps,
      SameSite = SameSiteMode.Lax,
      MaxAge = TimeSpan.FromMinutes(10),
      IsEssential = true,
      Path = "/api/auth/github/callback"
   };

   // Keep state and verifier server-inaccessible to JavaScript.
   context.Response.Cookies.Append(
      "github_oauth_state",
      state,
      cookieOptions
   );
   context.Response.Cookies.Append(
      "github_oauth_verifier",
      codeVerifier,
      cookieOptions
   );

   var parameters = new Dictionary<string, string?>
   {
      ["client_id"] = settings.ClientId,
      ["redirect_uri"] = settings.CallbackUrl,
      ["state"] = state,
      ["code_challenge"] = codeChallenge,
      ["code_challenge_method"] = "S256"
   };

   var authorizationUrl = QueryHelpers.AddQueryString(
      "https://github.com/login/oauth/authorize",
      parameters
   );

   return Results.Redirect(authorizationUrl);
});

app.Run();
