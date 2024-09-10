using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Server.AspNetCore;

namespace Backbone.ConsumerApi.InterBackboneCommunication;
public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Assuming you want to replace the user or set a new user.
        // Check the condition for replacing the user based on token, or custom logic.

        var fromBackbone = context.Request.Headers["X-From-Backbone"].FirstOrDefault();

        if (string.IsNullOrEmpty(fromBackbone))
        {
            await _next(context);
        }
        else
        {

            // Example: Create a new ClaimsIdentity and set it in HttpContext.User
            var claims = new List<Claim>
        {
            new Claim("address", context.Request.Headers["X-Identity-Proxy"].FirstOrDefault()!),
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
