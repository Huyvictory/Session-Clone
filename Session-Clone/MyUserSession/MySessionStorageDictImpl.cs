using System.Globalization;

namespace Session_Clone.MyUserSession;

public class MySessionStorageDictImpl : IMySessionStorage
{
    private readonly IMySessionStorageEngine _engine;
    private readonly Dictionary<string, MySession> _sessions = new();
    private readonly ILogger<MySession> _logger;
    private readonly Timer _cleanupTimer;

    public MySessionStorageDictImpl(IMySessionStorageEngine engine, ILogger<MySession> logger)
    {
        _engine = engine;
        _logger = logger;

        // Run cleanup every 1 minute
        _cleanupTimer = new Timer(CleanupExpiredSessions, null,
            TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        // Load existing sessions during initialization
        LoadExistingSessionsAsync().Wait();
    }

    public ISession Create()
    {
        var newSession = new MySession(Guid.NewGuid().ToString("N"), _engine, _logger);
        _sessions[newSession.Id] = newSession;
        return newSession;
    }

    public ISession Get(string sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var session) ? session : Create();
    }

    private async Task LoadExistingSessionsAsync()
    {
        try
        {
            // Get the Sessions directory path from the engine
            // We need to cast it to access the directoryPath
            if (_engine is not FileMySessionStorageEngine fileEngine)
            {
                _logger.LogWarning("Unable to load existing sessions - engine is not FileMySessionStorageEngine");
                return;
            }

            // Get all files in the Sessions directory
            var sessionFiles = Directory.GetFiles(fileEngine.DirectoryPath);

            foreach (var filePath in sessionFiles)
            {
                // Extract session ID from file name
                var sessionId = Path.GetFileName(filePath);

                // Validate if the file name is a valid GUID
                if (!Guid.TryParse(sessionId, out _)) continue;

                // Create new session instance
                var session = new MySession(sessionId, _engine, _logger);

                try
                {
                    // Load the session data
                    await session.LoadAsync();

                    _sessions[sessionId] = session;

                    _logger.LogInformation("Loaded existing session: {SessionId}", sessionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load session {SessionId}", sessionId);
                }
            }

            _logger.LogInformation("Loaded {Count} existing sessions", _sessions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load existing sessions");
        }
    }

    private void CleanupExpiredSessions(object state)
    {
        var expiredSessionIds = _sessions
            .Where(kvp => kvp.Value.IsExpired)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var sessionId in expiredSessionIds)
        {
            _sessions.Remove(sessionId);
            File.Delete(Path.Combine(((_engine as FileMySessionStorageEngine)!).DirectoryPath, sessionId));
        }
    }
}