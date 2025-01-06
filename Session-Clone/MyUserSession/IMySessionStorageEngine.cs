namespace Session_Clone.MyUserSession;

public interface IMySessionStorageEngine
{
    Task CommitAsync(string id, Dictionary<string, byte[]> sessionsStore, CancellationToken cancellationToken);

    Task<Dictionary<string, byte[]>> LoadAsync(string id, CancellationToken cancellationToken);
}