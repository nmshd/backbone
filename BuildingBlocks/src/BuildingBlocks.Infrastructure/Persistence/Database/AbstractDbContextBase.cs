using System.Data;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public class AbstractDbContextBase : DbContext, IDbContext
{
    private const int MAX_RETRY_COUNT = 50000;
    private const string SQLSERVER = "Microsoft.EntityFrameworkCore.SqlServer";
    private const string POSTGRES = "Npgsql.EntityFrameworkCore.PostgreSQL";
    private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromSeconds(1);
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider? _serviceProvider;

    protected AbstractDbContextBase()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        _eventBus = null!;
    }

    protected AbstractDbContextBase(DbContextOptions options, IEventBus eventBus, IServiceProvider? serviceProvider = null) : base(options)
    {
        _serviceProvider = serviceProvider;
        _eventBus = eventBus;
    }

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
                executionStrategy = new NpgsqlRetryingExecutionStrategy(this, MAX_RETRY_COUNT, MAX_RETRY_DELAY, errorNumbersToRetry?.ConvertAll(x => x.ToString()));
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
// #else
        // optionsBuilder.ConfigureWarnings(w => w.Log(evilEvents.Select(lle => (lle, Microsoft.Extensions.Logging.LogLevel.Warning)).ToArray()));
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Model.SetDbProvider(Database);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<IdentityAddress>().AreUnicode(false).AreFixedLength(false).HaveMaxLength(IdentityAddress.MAX_LENGTH).HaveConversion<IdentityAddressValueConverter>();
        configurationBuilder.Properties<DeviceId>().AreUnicode(false).AreFixedLength().HaveMaxLength(DeviceId.MAX_LENGTH).HaveConversion<DeviceIdValueConverter>();
        configurationBuilder.Properties<Username>().AreUnicode(false).AreFixedLength().HaveMaxLength(Username.MAX_LENGTH).HaveConversion<UsernameValueConverter>();

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
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
}
