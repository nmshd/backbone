using Backbone.AdminApi.Authentication;
using Backbone.AdminApi.Versions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/ValidateApiKey")]
[Authorize("ApiKey")]
[V1]
public class ApiKeyValidationController : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public IActionResult ValidateApiKey([FromBody] ValidateApiKeyRequest? request, [FromServices] ApiKeyValidator apiKeyValidator)
    {
        var apiKeyIsValid = apiKeyValidator.IsApiKeyValid(request?.ApiKey);
        return Ok(new { isValid = apiKeyIsValid });
    }

    public class ValidateApiKeyRequest
    {
        public string? ApiKey { get; set; }
    }
}
