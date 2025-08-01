using System.Data;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using OpenIddict.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database;

public class DevicesDbContext : IdentityDbContext<ApplicationUser>, IOpenIddictEntityFrameworkCoreContext
{
    private const int MAX_RETRY_COUNT = 50000;
    private const string SQLSERVER = "Microsoft.EntityFrameworkCore.SqlServer";
    private const string POSTGRES = "Npgsql.EntityFrameworkCore.PostgreSQL";
    private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromSeconds(1);
    private readonly IEventBus _eventBus;

    private readonly IServiceProvider? _serviceProvider;

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options) : base(options)
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        _eventBus = null!;
    }

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options, IEventBus eventBus) : base(options)
    {
        _eventBus = eventBus;
    }

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options)
    {
        _serviceProvider = serviceProvider;
        _eventBus = eventBus;
    }

    public DbSet<IdentityDeletionProcessAuditLogEntry> IdentityDeletionProcessAuditLogs { get; set; } = null!;

    public DbSet<Identity> Identities { get; set; } = null!;

    public DbSet<Device> Devices { get; set; } = null!;

    public DbSet<Challenge> Challenges { get; set; } = null!;

    public DbSet<Tier> Tiers { get; set; } = null!;

    public DbSet<PnsRegistration> PnsRegistrations { get; set; } = null!;

    public DbSet<FeatureFlag> FeatureFlags { get; set; } = null!;

    public DbSet<RelationshipTemplateAllocation> RelationshipTemplateAllocations { get; set; } = null!;

    public DbSet<Relationship> Relationships { get; set; } = null!;

    public IQueryable<T> SetReadOnly<T>() where T : class
    {
        return Set<T>().AsNoTracking();
    }

    public async Task RunInTransaction(Func<Task> action, List<int>? errorNumbersToRetry,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        ExecutionStrategy executionStrategy;
        switch (Database.ProviderName)
        {
            case SQLSERVER:
                executionStrategy = new SqlServerRetryingExecutionStrategy(this, MAX_RETRY_COUNT, MAX_RETRY_DELAY, errorNumbersToRetry);
                break;
            case POSTGRES:
                var errorCodesToRetry = errorNumbersToRetry != null ? errorNumbersToRetry.ConvertAll(x => x.ToString()) : [];
                executionStrategy = new NpgsqlRetryingExecutionStrategy(this, MAX_RETRY_COUNT, MAX_RETRY_DELAY, errorCodesToRetry);
                break;
            default:
                throw new Exception($"Unsupported database provider: {Database.ProviderName}");
        }

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(isolationLevel);
            await action();
            await transaction.CommitAsync();
        });
    }

    public async Task RunInTransaction(Func<Task> action, IsolationLevel isolationLevel)
    {
        await RunInTransaction(action, null, isolationLevel);
    }

    public async Task<T> RunInTransaction<T>(Func<Task<T>> action, List<int>? errorNumbersToRetry,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        // the '!' is safe here because the default value is only returned after the action is executed, which is setting the response
        var response = default(T)!;

        await RunInTransaction(async () => { response = await action(); }, errorNumbersToRetry, isolationLevel);

        return response;
    }

    public async Task<T> RunInTransaction<T>(Func<Task<T>> func, IsolationLevel isolationLevel)
    {
        return await RunInTransaction(func, null, isolationLevel);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entities = GetChangedEntities();
        var result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEvents(entities);

        return result;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (EnvironmentVariables.DEBUG_PERFORMANCE && _serviceProvider != null)
            optionsBuilder.AddInterceptors(_serviceProvider.GetRequiredService<SaveChangesTimeInterceptor>());

        optionsBuilder.UseLazyLoadingProxies();

        var evilEvents = new[]
        {
            RelationalEventId.MultipleCollectionIncludeWarning,

            CoreEventId.NavigationLazyLoading,
            CoreEventId.DetachedLazyLoadingWarning,
            CoreEventId.LazyLoadOnDisposedContextWarning
        };
#if DEBUG
        optionsBuilder.ConfigureWarnings(w => w.Throw(evilEvents));
#else
        optionsBuilder.ConfigureWarnings(w => w.Log(evilEvents.Select(lle => (lle, Microsoft.Extensions.Logging.LogLevel.Warning)).ToArray()));
#endif
    }

    public async Task<List<string>> GetFcmAppIdsForWhichNoConfigurationExists(ICollection<string> supportedAppIds)
    {
        return await GetAppIdsForWhichNoConfigurationExists("fcm", supportedAppIds);
    }

    public async Task<List<string>> GetApnsBundleIdsForWhichNoConfigurationExists(ICollection<string> supportedAppIds)
    {
        return await GetAppIdsForWhichNoConfigurationExists("apns", supportedAppIds);
    }

    public override int SaveChanges()
    {
        var entities = GetChangedEntities();
        var result = base.SaveChanges();
        PublishDomainEvents(entities).GetAwaiter().GetResult();

        return result;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var entities = GetChangedEntities();
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        PublishDomainEvents(entities).GetAwaiter().GetResult();

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        var entities = GetChangedEntities();
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await PublishDomainEvents(entities);

        return result;
    }

    private async Task<List<string>> GetAppIdsForWhichNoConfigurationExists(string platform, ICollection<string> supportedAppIds)
    {
        return await PnsRegistrations
            .AsNoTracking()
            .Where(x => ((string)(object)x.Handle).StartsWith(platform))
            .Where(x => !supportedAppIds.Contains(x.AppId))
            .Select(x => x.AppId)
            .Distinct()
            .ToListAsync();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<IdentityAddress>().AreUnicode(false).AreFixedLength(false)
            .HaveMaxLength(IdentityAddress.MAX_LENGTH).HaveConversion<IdentityAddressValueConverter>();
        configurationBuilder.Properties<DeviceId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(DeviceId.MAX_LENGTH).HaveConversion<DeviceIdValueConverter>();
        configurationBuilder.Properties<CommunicationLanguage>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(CommunicationLanguage.LENGTH).HaveConversion<CommunicationLanguageEntityFrameworkValueConverter>();
        configurationBuilder.Properties<DevicePushIdentifier>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(DevicePushIdentifier.MAX_LENGTH).HaveConversion<DevicePushIdentifierEntityFrameworkValueConverter>();
        configurationBuilder.Properties<Username>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(Username.MAX_LENGTH).HaveConversion<UsernameValueConverter>();
        configurationBuilder.Properties<TierId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(TierId.MAX_LENGTH).HaveConversion<TierIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<TierName>().AreUnicode().AreFixedLength(false)
            .HaveMaxLength(TierName.MAX_LENGTH).HaveConversion<TierNameEntityFrameworkValueConverter>();
        configurationBuilder.Properties<IdentityDeletionProcessId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(IdentityDeletionProcessId.MAX_LENGTH).HaveConversion<IdentityDeletionProcessIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<IdentityDeletionProcessAuditLogEntryId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(IdentityDeletionProcessAuditLogEntryId.MAX_LENGTH).HaveConversion<IdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<PnsHandle>().AreUnicode().AreFixedLength(false)
            .HaveMaxLength(200).HaveConversion<PnsHandleEntityFrameworkValueConverter>();
        configurationBuilder.Properties<FeatureFlagName>().AreUnicode().AreFixedLength(false)
            .HaveMaxLength(FeatureFlagName.MAX_LENGTH).HaveConversion<FeatureFlagNameEntityFrameworkValueConverter>();

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Devices");

        builder.ApplyConfigurationsFromAssembly(typeof(DeviceEntityTypeConfiguration).Assembly);
    }

    private List<Entity> GetChangedEntities() => ChangeTracker
        .Entries()
        .Where(x => x.Entity is Entity)
        .Select(x => (Entity)x.Entity)
        .ToList();

    private async Task PublishDomainEvents(List<Entity> entities)
    {
        foreach (var e in entities)
        {
            await _eventBus.Publish(e.DomainEvents);
            e.ClearDomainEvents();
        }
    }

    public ValueTask<DbContext> GetDbContextAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<DbContext>(this);
    }
}
