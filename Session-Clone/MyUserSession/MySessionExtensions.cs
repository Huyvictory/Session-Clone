namespace Session_Clone.MyUserSession;

public static class MySessionExtensions
{
    private const string SessionIdCookieName = "MY_SESSION_ID";

    public static ISession GetMyUserSession(this HttpContext context)
    {
        MySessionScopeContainer? sessionContainer =
            context.RequestServices.GetRequiredService<MySessionScopeContainer>();

        if (sessionContainer.Session != null)
        {
            return sessionContainer.Session;
        }

        string? sessionId = context.Request.Cookies[SessionIdCookieName];
        ISession session;

        session = IsSessionIdFormatValid(sessionId)
            ? context.RequestServices.GetRequiredService<IMySessionStorage>().Get(sessionId!)
            : context.RequestServices.GetRequiredService<IMySessionStorage>().Create();

        context.Response.Cookies.Append(SessionIdCookieName, session.Id, new CookieOptions()
        {
            HttpOnly = true,
        });

        sessionContainer.Session = session;

        return session;
    }

    private static bool IsSessionIdFormatValid(string? sessionId)
    {
        return !string.IsNullOrEmpty(sessionId) && Guid.TryParse(sessionId, out _);
    }
}