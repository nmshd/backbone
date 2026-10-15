using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.DatabaseMigrator;

// Keep migration scaffolding independent of the executable migrator's startup and databases.
public abstract class DesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
{
    public T CreateDbContext(string[] args)
    {
        var provider = Environment.GetEnvironmentVariable("Infrastructure__SqlDatabase__Provider") ?? DatabaseConfiguration.POSTGRES;
        var services = new ServiceCollection();
        services.AddSingleton<IEventBus, DummyEventBus>();
        services.AddAllDbContexts(new DatabaseConfiguration
        {
            Provider = provider,
            ConnectionString = provider == DatabaseConfiguration.POSTGRES
                ? "Host=localhost;Database=design;Username=design"
                : "Server=localhost;Database=design;Integrated Security=true;TrustServerCertificate=true"
        });
        // EF tooling owns the returned context; no application host or database connection is started.
        return services.BuildServiceProvider().GetRequiredService<T>();
    }
}

public sealed class AnnouncementsDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Announcements.Infrastructure.Persistence.Database.AnnouncementsDbContext>;
public sealed class ChallengesDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Challenges.Infrastructure.Persistence.Database.ChallengesDbContext>;
public sealed class DevicesDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Devices.Infrastructure.Persistence.Database.DevicesDbContext>;
public sealed class FilesDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Files.Infrastructure.Persistence.Database.FilesDbContext>;
public sealed class MessagesDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Messages.Infrastructure.Persistence.Database.MessagesDbContext>;
public sealed class QuotasDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QuotasDbContext>;
public sealed class RelationshipsDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Relationships.Infrastructure.Persistence.Database.RelationshipsDbContext>;
public sealed class SynchronizationDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.SynchronizationDbContext>;
public sealed class TokensDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.Modules.Tokens.Infrastructure.Persistence.Database.TokensDbContext>;
public sealed class AdminApiDesignTimeDbContextFactory : DesignTimeDbContextFactory<Backbone.AdminApi.Infrastructure.Persistence.Database.AdminApiDbContext>;
