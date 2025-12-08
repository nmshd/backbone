using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class DatawalletQueryableExtensions
{
    extension(IQueryable<Datawallet> query)
    {
        public async Task<Datawallet?> OfOwner(IdentityAddress owner, CancellationToken cancellationToken)
        {
            return await query.FirstOrDefaultAsync(e => e.Owner == owner, cancellationToken);
        }

        public IQueryable<Datawallet> WithLatestModification(IdentityAddress owner)
        {
            return query.Include(dw => dw.Modifications.Where(m => m.CreatedBy == owner).OrderByDescending(m => m.Index).Take(1));
        }
    }
}
