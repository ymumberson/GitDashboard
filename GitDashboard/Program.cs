using GitDashboard.Exceptions;
using GitDashboard.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<InvalidRepositoryResponseHandler>();

builder.Services.AddSingleton<IGitRunner, GitRunner>();
builder.Services.AddSingleton<IGitService, GitService>();
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
   RepositoryAnalysisService service) =>
{
   if (string.IsNullOrWhiteSpace(path))
   {
      return Results.BadRequest(new ProblemDetails
      {
         Status = StatusCodes.Status400BadRequest,
         Title = "Invalid request",
         Detail = "The repository path is required"
      });
   }
   
   return Results.Ok(await service.AnalyseAsync(path));
});

app.Run();
