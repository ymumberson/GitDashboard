using GitDashboard.Models;

namespace GitDashboard.Services;

public interface IRepositorySourceResolver
{
    IRepositorySource Resolve(RepositoryReference repository);
}