using AdminUi.Infrastructure.DTOs;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace AdminUi.Infrastructure.Persistence.Database;

public class AdminUiDbContext : AbstractDbContextBase
{
    public AdminUiDbContext()
    {
    }

    public AdminUiDbContext(DbContextOptions<AdminUiDbContext> options) : base(options)
    {
    }

    public AdminUiDbContext(DbContextOptions<AdminUiDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }

    public DbSet<IdentityOverview> IdentityOverviews { get; set; }

    public DbSet<TierOverview> TierOverviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AdminUiDbContext).Assembly);
    }
}
