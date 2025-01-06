namespace Session_Clone.MyUserSession;

public class MySession(string id, IMySessionStorageEngine engine) : ISession
{
    private readonly Dictionary<string, byte[]> _sessions_store = new();

    public bool IsAvailable
    {
        get
        {
            LoadAsync(CancellationToken.None).Wait();
            return true;
        }
    }

    public string Id => id;

    public IEnumerable<string> Keys => _sessions_store.Keys;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await engine.CommitAsync(id, _sessions_store, cancellationToken);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        _sessions_store.Clear();

        var loadedSessionStorage = await engine.LoadAsync(id, cancellationToken);

        foreach (var (key, value) in loadedSessionStorage)
        {
            _sessions_store[key] = value;
        }
    }

    public bool TryGetValue(string key, out byte[]? value)
    {
        return _sessions_store.TryGetValue(key, out value);
    }

    public void Set(string key, byte[] value)
    {
        _sessions_store[key] = value;
    }

    public void Remove(string key)
    {
        _sessions_store.Remove(key);
    }

    public void Clear()
    {
        _sessions_store.Clear();
    }
}