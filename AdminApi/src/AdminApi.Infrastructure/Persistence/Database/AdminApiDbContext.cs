using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database;

public class AdminApiDbContext : AbstractDbContextBase
{
    public AdminApiDbContext()
    {
    }

    public AdminApiDbContext(DbContextOptions<AdminApiDbContext> options) : base(options)
    {
    }

    public AdminApiDbContext(DbContextOptions<AdminApiDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }

    public DbSet<IdentityOverview> IdentityOverviews { get; set; } = null!;

    public DbSet<TierOverview> TierOverviews { get; set; } = null!;

    public DbSet<ClientOverview> ClientOverviews { get; set; } = null!;

    public DbSet<RelationshipOverview> RelationshipOverviews { get; set; } = null!;

    public DbSet<MessageOverview> MessageOverviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("AdminUi");

        builder.ApplyConfigurationsFromAssembly(typeof(AdminApiDbContext).Assembly);
    }
}
