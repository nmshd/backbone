using Backbone.AdminApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/ValidateApiKey")]
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
