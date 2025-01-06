namespace Session_Clone.MyUserSession;

public class MySessionStorage : IMySessionStorage
{
    private readonly IMySessionStorageEngine engine;
    private readonly Dictionary<string, MySession> sessions = new();

    public MySessionStorage(IMySessionStorageEngine engine)
    {
        this.engine = engine;
    }

    public ISession Create()
    {
        var newSession = new MySession(Guid.NewGuid().ToString("N"), engine);
        sessions[newSession.Id] = newSession;
        return newSession;
    }

    public ISession Get(string sessionId)
    {
        return sessions.TryGetValue(sessionId, out var session) ? session : Create();
    }
}