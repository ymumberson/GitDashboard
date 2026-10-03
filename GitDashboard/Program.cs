using GitDashboard.Exceptions;
using GitDashboard.Services;
using GitDashboard.Configuration;
using GitDashboard.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddOptions<GitOptions>()
   .Bind(builder.Configuration.GetSection("Git"))
   .ValidateDataAnnotations()
   .ValidateOnStart();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<InvalidRepositoryResponseHandler>();

builder.Services.AddSingleton<IGitRunner, GitRunner>();
builder.Services.AddSingleton<IGitService, GitService>();
builder.Services.AddSingleton<IRepositorySource, LocalRepositorySource>();
builder.Services.AddHttpClient<GitHubRepositorySource>(client =>
{
   client.BaseAddress = new Uri("https://api.github.com");

   client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(
            "application/vnd.github+json"));

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "GitDashboard");
});
builder.Services.AddSingleton<IRepositorySource>(sp =>
    sp.GetRequiredService<GitHubRepositorySource>());
builder.Services.AddSingleton<IRepositorySourceResolver, RepositorySourceResolver>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
builder.Services.AddSingleton<RepositoryAnalysisService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseExceptionHandler();

app.MapGet("/api/hello", () =>
{
   return new
   {
       message = "Hello world!"
   };
});

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

app.Run();
