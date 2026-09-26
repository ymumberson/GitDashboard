using GitDashboard.Exceptions;
using GitDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGitRunner, GitRunner>();
builder.Services.AddSingleton<IGitService, GitService>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
builder.Services.AddSingleton<RepositoryAnalysisService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/hello", () =>
{
   return new
   {
       message = "Hello world!"
   };
});

app.MapGet("api/analyse", async (
   string path,
   RepositoryAnalysisService repositoryAnalysisService) =>
{
   try
   {
      return Results.Ok(await repositoryAnalysisService.AnalyseAsync(path));
   } 
   catch (InvalidRepositoryException e)
   {
      return Results.BadRequest(new {error = e.Message});
   }
});

app.Run();
