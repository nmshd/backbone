using Backbone.AdminApi.Versions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/xsrf")]
[Authorize("ApiKey")]
[ApiController]
[V1]
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
