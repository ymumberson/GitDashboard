using System.Collections.Concurrent;

namespace GitDashboard.Authentication;

/// <summary>
/// In-memory storage of session tokens. Volatile, so tokens are lost on application restart.
/// </summary>
public sealed class GitHubUserTokenStore
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();

    public void Set(string sessionId, string accessToken)
    {
        _tokens[sessionId] = accessToken;
    }

    public bool TryGet(string sessionId, out string accessToken)
    {
        return _tokens.TryGetValue(sessionId, out accessToken!);
    }

    public void Remove(string sessionId)
    {
        _tokens.TryRemove(sessionId, out _);
    }
}