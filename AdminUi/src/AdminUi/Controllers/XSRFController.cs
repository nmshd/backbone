using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUi.Controllers;

[Route("api/v1/xsrf")]
public class XsrfController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public IActionResult SetXsrfToken([FromServices] IAntiforgery antiForgery)
    {
        var token = antiForgery.GetAndStoreTokens(Request.HttpContext);
        return Ok(token.RequestToken);
    }
}
