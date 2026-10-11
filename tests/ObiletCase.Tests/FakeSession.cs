using Microsoft.AspNetCore.Http;

namespace ObiletCase.Tests;

/// <summary>A visitor's ASP.NET session kept in a dictionary, so tests do not need a running web server.</summary>
public class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _values = new();

    public string Id
    {
        get { return "fake-session"; }
    }

    public bool IsAvailable
    {
        get { return true; }
    }

    public IEnumerable<string> Keys
    {
        get { return _values.Keys; }
    }

    public void Set(string key, byte[] value)
    {
        _values[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        return _values.TryGetValue(key, out value!);
    }

    public void Remove(string key)
    {
        _values.Remove(key);
    }

    public void Clear()
    {
        _values.Clear();
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
