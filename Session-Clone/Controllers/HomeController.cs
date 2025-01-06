namespace Session_Clone.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> IndexAsync()
    {
        var session = HttpContext.GetMyUserSession();

        session.SetString("name", "John Doe");
        await session.CommitAsync();

        return View("Index");
    }

    public async Task<IActionResult> PrivacyAsync()
    {
        var session = HttpContext.GetMyUserSession();
        await session.LoadAsync();

        var name_session = session.GetString("name");

        return View("Privacy", name_session);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}