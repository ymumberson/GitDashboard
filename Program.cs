using GitDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GitService>();

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

app.MapGet("api/analyse", async (string path, GitService gitService) =>
{
   var commits = await gitService.GetCommitsAsync(path);

   return commits;
});

app.Run();
