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
            .AsNoTracking()
            .Include(u => u.Device)
            .ThenInclude(d => d.Identity)
            .FirstOrDefaultAsync(filter, cancellationToken);

        return user;
    }

    public override Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        Context.Attach(user.Device);
        Context.Update(user.Device);
        return base.UpdateAsync(user, cancellationToken);
    }
}
