using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;

public class CustomOpenIddictEntityFrameworkCoreApplicationStore :
    OpenIddictEntityFrameworkCoreApplicationStore<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken, string>
{
    private readonly DevicesDbContext _customDbContext;

    public CustomOpenIddictEntityFrameworkCoreApplicationStore(
        IMemoryCache cache,
        DevicesDbContext context,
        IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions> options)
        : base(cache, context, options)
    {
        this._customDbContext = context;
    }

    public override async ValueTask DeleteAsync(CustomOpenIddictEntityFrameworkCoreApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);

        await _customDbContext.RunInTransaction(async () =>
        {
            // Remove all the authorizations associated with the application and
            // the tokens attached to these implicit or explicit authorizations.
            var authorizations = await ListAuthorizationsAsync();
            foreach (var authorization in authorizations)
            {
                foreach (var token in authorization.Tokens)
                {
                    _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreToken>().Remove(token);
                }

                _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreAuthorization>().Remove(authorization);
            }

            // Remove all the tokens associated with the application.
            var tokens = await ListTokensAsync();
            foreach (var token in tokens)
            {
                _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreToken>().Remove(token);
            }

            _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreApplication>().Remove(application);

            try
            {
                await _customDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                // Reset the state of the entity to prevents future calls to SaveChangesAsync() from failing.
                _customDbContext.Entry(application).State = EntityState.Unchanged;

                foreach (var authorization in authorizations)
                {
                    _customDbContext.Entry(authorization).State = EntityState.Unchanged;
                }

                foreach (var token in tokens)
                {
                    _customDbContext.Entry(token).State = EntityState.Unchanged;
                }

                throw new OpenIddictExceptions.ConcurrencyException(
                    "The application was concurrently updated and cannot be persisted in its current state.\r\nReload the application from the database and retry the operation.",
                    exception);
            }
        }, []);
        return;

        Task<List<CustomOpenIddictEntityFrameworkCoreToken>> ListTokensAsync()
            => _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreToken>()
                .Where(t => t.Authorization == null)
                .Where(t => t.Application!.Id == application.Id)
                .ToListAsync(cancellationToken);

        Task<List<CustomOpenIddictEntityFrameworkCoreAuthorization>> ListAuthorizationsAsync()
            => _customDbContext.Set<CustomOpenIddictEntityFrameworkCoreAuthorization>()
                .Include(a => a.Tokens)
                .Where(a => a.Application!.Id == application.Id)
                .ToListAsync(cancellationToken);
    }
}
