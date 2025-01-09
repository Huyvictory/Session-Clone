namespace Session_Clone.MyUserSession;

public class MySessionScopeContainer
{
    public MySessionScopeContainer(ILogger<MySessionScopeContainer> logger)
    {
        logger.LogInformation("MySessionScopeContainer created");
    }

    public ISession? Session { get; set; }
}