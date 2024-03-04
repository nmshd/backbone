using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Authentication;

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly ApiKeyValidator _apiKeyValidator;
    private const string API_KEY_HEADER_NAME = "X-API-KEY";

    public ApiKeyAuthenticationSchemeHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ApiKeyValidator apiKeyValidator) : base(options, logger, encoder)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Context.Request.Headers[API_KEY_HEADER_NAME];

        if (!_apiKeyValidator.IsApiKeyValid(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Invalid {API_KEY_HEADER_NAME}"));
        }
        var claims = new[] { new Claim(ClaimTypes.Name, "VALID USER") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
