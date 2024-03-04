using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Controller]
[Route("")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectPermanent("index.html");
    }
}
