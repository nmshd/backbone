using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace Devices.API;

public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients())
            {
                context.Clients.Add(client.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.GetIdentitityResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var resource in Config.GetApiResources())
            {
                context.ApiResources.Add(resource.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var apiScope in Config.GetApiScopes())
            {
                context.ApiScopes.Add(apiScope.ToEntity());
            }

            await context.SaveChangesAsync();
        }
    }
}
