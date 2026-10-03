using GitDashboard.Models;

namespace GitDashboard.Services;

public class RepositorySourceResolver : IRepositorySourceResolver
{
    private readonly IEnumerable<IRepositorySource> _sources;

    public RepositorySourceResolver(IEnumerable<IRepositorySource> sources)
    {
        _sources = sources;
    }

    public IRepositorySource Resolve(RepositoryReference repository)
    {
        var source = _sources.FirstOrDefault(
            source => source.CanHandle(repository)
        );

        if (source is null)
        {
            throw new InvalidOperationException("No repository source can handle this repository.");
        }

        return source;
    }
}