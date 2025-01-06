namespace Session_Clone.MyUserSession;

public interface IMySessionStorage
{
    ISession Create();

    ISession Get(string sessionId);
}