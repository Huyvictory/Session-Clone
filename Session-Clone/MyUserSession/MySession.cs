namespace Session_Clone.MyUserSession;

public class MySession : ISession
{
    public MySession(string id, IMySessionStorageEngine engine, ILogger<MySession> logger)
    {
        Id = id;
        this.engine = engine;
        _lastAccessed = DateTime.Now;
        _logger = logger;
    }

    private readonly ILogger<MySession> _logger;

    private readonly Dictionary<string, byte[]> _sessions_store = new();

    public bool IsAvailable
    {
        get
        {
            Load();
            UpdateLastAccessed();
            return true;
        }
    }

    public string Id { get; }
    private readonly IMySessionStorageEngine engine;

    public IEnumerable<string> Keys => _sessions_store.Keys;

    public DateTime _lastAccessed;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public bool IsExpired => DateTime.UtcNow - _lastAccessed > _timeout;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await engine.CommitAsync(Id, _sessions_store, cancellationToken);
    }

    public void Load()
    {
        _sessions_store.Clear();

        var loadedSessionStorage = engine.Load(Id);

        foreach (var (key, value) in loadedSessionStorage)
        {
            _sessions_store[key] = value;
        }

        UpdateLastAccessed();
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        _sessions_store.Clear();

        var loadedSessionStorage = await engine.LoadAsync(Id, cancellationToken);

        foreach (var (key, value) in loadedSessionStorage)
        {
            _sessions_store[key] = value;
        }

        UpdateLastAccessed();
    }

    public bool TryGetValue(string key, out byte[]? value)
    {
        UpdateLastAccessed();
        return _sessions_store.TryGetValue(key, out value);
    }

    public void Set(string key, byte[] value)
    {
        UpdateLastAccessed();
        _sessions_store[key] = value;
    }

    public void Remove(string key)
    {
        _sessions_store.Remove(key);
        UpdateLastAccessed();
    }

    public void Clear()
    {
        _sessions_store.Clear();
        UpdateLastAccessed();
    }

    private void UpdateLastAccessed()
    {
        var previousAccess = _lastAccessed;
        _lastAccessed = DateTime.UtcNow;

        _logger.LogInformation(
            "Session {SessionId} accessed. Previous access: {PreviousAccess}, New access: {NewAccess}",
            Id,
            previousAccess,
            _lastAccessed
        );
    }
}