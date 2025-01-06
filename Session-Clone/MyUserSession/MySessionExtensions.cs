namespace Session_Clone.MyUserSession;

public static class MySessionExtensions
{
    private const string SessionIdCookieName = "MY_SESSION_ID";

    public static ISession GetMyUserSession(this HttpContext context)
    {
        string? sessionId = context.Request.Cookies[SessionIdCookieName];
        ISession session;

        session = IsSessionIdFormatValid(sessionId)
            ? context.RequestServices.GetRequiredService<IMySessionStorage>().Get(sessionId!)
            : context.RequestServices.GetRequiredService<IMySessionStorage>().Create();

        context.Response.Cookies.Append(SessionIdCookieName, session.Id);
        return session;
    }

    private static bool IsSessionIdFormatValid(string? sessionId)
    {
        return !string.IsNullOrEmpty(sessionId) && Guid.TryParse(sessionId, out _);
    }
}