using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;

public class CustomOpenIddictEntityFrameworkCoreApplicationStore :
    OpenIddictEntityFrameworkCoreApplicationStore<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken, DevicesDbContext, string>
{
    public CustomOpenIddictEntityFrameworkCoreApplicationStore(
        IMemoryCache cache,
        DevicesDbContext context,
        IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions> options)
        : base(cache, context, options)
    {
    }

    public override async ValueTask DeleteAsync(CustomOpenIddictEntityFrameworkCoreApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        Task<List<CustomOpenIddictEntityFrameworkCoreAuthorization>> ListAuthorizationsAsync()
            => Context.Set<CustomOpenIddictEntityFrameworkCoreAuthorization>()
                .Include(a => a.Tokens)
                .Where(a => a.Application!.Id == application.Id)
                .ToListAsync(cancellationToken);

        Task<List<CustomOpenIddictEntityFrameworkCoreToken>> ListTokensAsync()
            => Context.Set<CustomOpenIddictEntityFrameworkCoreToken>()
                .Where(t => t.Authorization == null)
                .Where(t => t.Application!.Id == application.Id)
                .ToListAsync(cancellationToken);

        await Context.RunInTransaction(async () =>
        {
            // Remove all the authorizations associated with the application and
            // the tokens attached to these implicit or explicit authorizations.
            var authorizations = await ListAuthorizationsAsync();
            foreach (var authorization in authorizations)
            {
                foreach (var token in authorization.Tokens)
                {
                    Context.Set<CustomOpenIddictEntityFrameworkCoreToken>().Remove(token);
                }

                Context.Set<CustomOpenIddictEntityFrameworkCoreAuthorization>().Remove(authorization);
            }

            // Remove all the tokens associated with the application.
            var tokens = await ListTokensAsync();
            foreach (var token in tokens)
            {
                Context.Set<CustomOpenIddictEntityFrameworkCoreToken>().Remove(token);
            }

            Context.Set<CustomOpenIddictEntityFrameworkCoreApplication>().Remove(application);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                // Reset the state of the entity to prevents future calls to SaveChangesAsync() from failing.
                Context.Entry(application).State = EntityState.Unchanged;

                foreach (var authorization in authorizations)
                {
                    Context.Entry(authorization).State = EntityState.Unchanged;
                }

                foreach (var token in tokens)
                {
                    Context.Entry(token).State = EntityState.Unchanged;
                }

                throw new OpenIddictExceptions.ConcurrencyException(
                    "The application was concurrently updated and cannot be persisted in its current state.\r\nReload the application from the database and retry the operation.",
                    exception);
            }
        }, new List<int>());
    }

}

