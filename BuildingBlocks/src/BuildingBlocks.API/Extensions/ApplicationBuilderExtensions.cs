using Enmeshed.BuildingBlocks.API.Mvc.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace Enmeshed.BuildingBlocks.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestResponseTimeMiddleware>();
            app.UseMiddleware<ResponseDurationMiddleware>();
            app.UseMiddleware<RequestIdMiddleware>();

            app.UseSecurityHeaders(policies =>
                policies
                    .AddDefaultSecurityHeaders()
                    .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
                    .AddCustomHeader("X-Frame-Options", "Deny")
            );

            app.UseRouting();

            app.UseCors();

            if (env.IsDevelopment()) IdentityModelEventSource.ShowPII = true;

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}