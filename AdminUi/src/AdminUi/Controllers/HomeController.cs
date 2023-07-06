using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUi.Controllers;

[Controller]
[Route("")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectPermanent("index.html");
    }
}
