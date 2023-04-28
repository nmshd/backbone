using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

public class QuotasDbContext : AbstractDbContextBase
{
    public QuotasDbContext(DbContextOptions<QuotasDbContext> options) : base(options) { }

    public DbSet<Identity> Identities { get; set; }

    public DbSet<Tier> Tiers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(QuotasDbContext).Assembly);
    }
}