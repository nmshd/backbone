using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;
using Backbone.AdminApi.Infrastructure.Persistence.Models.Relationships;
using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Backbone.AdminApi.Infrastructure.Persistence.Models.Tokens;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using File = Backbone.AdminApi.Infrastructure.Persistence.Models.Files.File;

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

    public DbSet<Identity> Identities { get; set; } = null!;
    public DbSet<OpenIddictApplication> OpenIddictApplications { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<Tier> Tiers { get; set; } = null!;
    public DbSet<PnsRegistration> PnsRegistrations { get; set; } = null!;
    public DbSet<IdentityDeletionProcessAuditLogEntry> IdentityDeletionProcessAuditLogs { get; set; } = null!;

    public DbSet<Relationship> Relationships { get; set; } = null!;
    public DbSet<RelationshipTemplate> RelationshipTemplates { get; set; } = null!;

    public DbSet<File> FileMetadata { get; set; } = null!;

    public DbSet<Message> Messages { get; set; } = null!;

    public DbSet<Datawallet> Datawallets { get; set; } = null!;
    public DbSet<DatawalletModification> DatawalletModifications { get; set; } = null!;
    public DbSet<SyncError> SyncErrors { get; set; } = null!;

    public DbSet<Token> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("AdminUi");

        builder.ApplyConfigurationsFromAssembly(typeof(AdminApiDbContext).Assembly);
    }
}
