using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Server.AspNetCore;

namespace Backbone.ConsumerApi.InterBackboneCommunication;
public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuthenticationService _authenticationService;

    public CustomAuthenticationMiddleware(RequestDelegate next, IAuthenticationService authenticationService)
    {
        _next = next;
        _authenticationService = authenticationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Assuming you want to replace the user or set a new user.
        // Check the condition for replacing the user based on token, or custom logic.

        var fromBackbone = context.Request.Headers["X-From-Backbone"].FirstOrDefault();

        if (string.IsNullOrEmpty(fromBackbone))
        {
            var authenticateResult = await _authenticationService.AuthenticateAsync(context, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (authenticateResult.Succeeded)
            {
                context.User = authenticateResult.Principal;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }
        else
        {

            // Example: Create a new ClaimsIdentity and set it in HttpContext.User
            var claims = new List<Claim>
        {
            new Claim("address", "newUser"),
            new Claim(ClaimTypes.Role, "Admin")
        };
            var identity = new ClaimsIdentity(claims, "Custom");
            var principal = new ClaimsPrincipal(identity);

            // Set the new principal to HttpContext.User
            context.User = principal;

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
