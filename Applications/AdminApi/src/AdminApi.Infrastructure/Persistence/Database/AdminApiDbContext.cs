using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database;

public class AdminApiDbContext : AbstractDbContextBase
{
    public AdminApiDbContext()
    {
    }

    public AdminApiDbContext(DbContextOptions<AdminApiDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public AdminApiDbContext(DbContextOptions<AdminApiDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public DbSet<IdentityOverview> IdentityOverviews { get; set; } = null!;

    public DbSet<TierOverview> TierOverviews { get; set; } = null!;

    public DbSet<ClientOverview> ClientOverviews { get; set; } = null!;

    public DbSet<RelationshipOverview> RelationshipOverviews { get; set; } = null!;

    public DbSet<MessageOverview> MessageOverviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("AdminUi");

        builder.ApplyConfigurationsFromAssembly(typeof(AdminApiDbContext).Assembly);
    }
}
