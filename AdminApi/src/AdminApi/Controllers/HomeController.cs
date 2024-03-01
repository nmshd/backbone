using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Controller]
[Route("")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectPermanent("index.html");
    }
}
