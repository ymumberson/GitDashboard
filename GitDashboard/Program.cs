using GitDashboard.Exceptions;
using GitDashboard.Services;
using GitDashboard.Configuration;

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
   RepositoryAnalysisService service,
   CancellationToken cancellationToken) =>
{
   if (string.IsNullOrWhiteSpace(path))
   {
      return Results.Problem(
         statusCode: StatusCodes.Status400BadRequest,
         title: "Invalid request",
         detail: "The repository path is required"
      );
   }
   
   return Results.Ok(await service.AnalyseAsync(path, cancellationToken));
});

app.Run();
