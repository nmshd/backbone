using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace AdminUi.OpenIddict;

public class CustomOpenIddictEntityFrameworkCoreApplicationStore :
    OpenIddictEntityFrameworkCoreApplicationStore<DevicesDbContext>
{
    public CustomOpenIddictEntityFrameworkCoreApplicationStore(
        IMemoryCache cache,
        DevicesDbContext context,
        IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions> options)
        : base(cache, context, options)
    {
    }

    public override async ValueTask DeleteAsync(OpenIddictEntityFrameworkCoreApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        Task<List<OpenIddictEntityFrameworkCoreAuthorization>> ListAuthorizationsAsync()
            => Context.Set<OpenIddictEntityFrameworkCoreAuthorization>()
                .Include(a => a.Tokens)
                .Where(a => a.Application!.Id == application.Id)
                .ToListAsync(cancellationToken);

        Task<List<OpenIddictEntityFrameworkCoreToken>> ListTokensAsync()
            => Context.Set<OpenIddictEntityFrameworkCoreToken>()
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
                    Context.Set<OpenIddictEntityFrameworkCoreToken>().Remove(token);
                }

                Context.Set<OpenIddictEntityFrameworkCoreAuthorization>().Remove(authorization);
            }

            // Remove all the tokens associated with the application.
            var tokens = await ListTokensAsync();
            foreach (var token in tokens)
            {
                Context.Set<OpenIddictEntityFrameworkCoreToken>().Remove(token);
            }

            Context.Set<OpenIddictEntityFrameworkCoreApplication>().Remove(application);

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

