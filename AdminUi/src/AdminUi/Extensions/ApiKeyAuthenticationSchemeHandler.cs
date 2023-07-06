using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AdminUi.Extensions;

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    public ApiKeyAuthenticationSchemeHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Context.Request.Headers["X-API-KEY"];

        if (!string.IsNullOrEmpty(Options.ApiKey) && apiKey != Options.ApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid X-API-KEY"));
        }
        var claims = new[] { new Claim(ClaimTypes.Name, "VALID USER") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}