namespace Session_Clone.Controllers;

public class TestSessionController : Controller
{
    public IActionResult TestGetSession()
    {
        var session = HttpContext.GetMyUserSession();

        session.SetString("Name", "Tom Hanks");

        session = HttpContext.GetMyUserSession();
        var newName = session.GetString("Name");

        if (newName is "Tom Hanks")
        {
            return Content("Session new name is " + newName);
        }

        return BadRequest("Session new name is not Tom Hanks");
    }

    public async Task<IActionResult> SetSessionValue(string key, string value)
    {
        var session = HttpContext.GetMyUserSession();
        await session.LoadAsync();

        session.SetString(key, value);

        await session.CommitAsync();

        return Ok();
    }

    public async Task<IActionResult> GetSessionValue(string key)
    {
        var session = HttpContext.GetMyUserSession();
        await session.LoadAsync();
        var value = session.GetString(key);

        return Ok(value);
    }
}