using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminUi.Controllers;

[Controller]
[Route("")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectPermanent("index.html");
    }
}
