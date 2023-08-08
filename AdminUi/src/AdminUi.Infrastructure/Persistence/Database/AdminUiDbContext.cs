using AdminUi.Infrastructure.DTOs;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace AdminUi.Infrastructure.Persistence.Database;

public class AdminUiDbContext : AbstractDbContextBase
{
    public AdminUiDbContext(DbContextOptions<AdminUiDbContext> options) : base(options) { }

    public DbSet<IdentityOverview> IdentityOverviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AdminUiDbContext).Assembly);
    }
}
