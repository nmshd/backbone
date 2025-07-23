using System.Linq.Expressions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.API.AspNetCoreIdentityCustomizations;

public class CustomUserStore : UserStore<ApplicationUser>
{
    public CustomUserStore(DevicesDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }

    public override async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await FindUser(u => u.Id == userId, cancellationToken);
        return user;
    }

    public override async Task<ApplicationUser?> FindByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await FindUser(u => u.NormalizedUserName == userName, cancellationToken);
        return user;
    }

    private async Task<ApplicationUser?> FindUser(Expression<Func<ApplicationUser, bool>> filter, CancellationToken cancellationToken)
    {
        var user = await Context
            .Set<ApplicationUser>()
            .Include(u => u.Device)
            .ThenInclude(d => d.Identity)
            .FirstOrDefaultAsync(filter, cancellationToken);

        return user;
    }

    public override async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        // This implementation is almost the same as the base implementation. The only difference is that we don't call `Context.Update`. 
        // That's not necessary because the user is already being tracked by the DbContext when it is retrieved from the database.
        // If we called `Context.Update` here, it would cause all properties to be marked as modified, instead of only the ones that changed.
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        Context.Attach(user);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }
}
