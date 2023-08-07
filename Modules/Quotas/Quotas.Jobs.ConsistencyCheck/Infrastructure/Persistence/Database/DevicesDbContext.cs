using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain.Aggregates.Devices;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database;

public class DevicesDbContext : AbstractDbContextBase
{
    public DevicesDbContext(DbContextOptions<DevicesDbContext> options) : base(options)
    {
    }

    public DbSet<Identity> Identities { get; set; }

    public DbSet<Tier> Tiers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DevicesDbContext).Assembly);
    }
}
