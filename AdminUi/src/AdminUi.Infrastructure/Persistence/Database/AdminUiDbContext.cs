using Backbone.AdminUi.Infrastructure.DTOs;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database;

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

    public DbSet<ClientOverview> ClientOverviews { get; set; }

    public DbSet<RelationshipOverview> RelationshipOverviews { get; set; }

    public DbSet<MessageOverview> MessageOverviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AdminUiDbContext).Assembly);
    }
}
