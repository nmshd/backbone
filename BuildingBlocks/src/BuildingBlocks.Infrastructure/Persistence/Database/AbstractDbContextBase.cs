using System.Data;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
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
    private readonly OutboxSaveChangesInterceptor _outbox = new();
    private readonly IServiceProvider? _serviceProvider;

    protected AbstractDbContextBase()
    {
    }

    protected AbstractDbContextBase(DbContextOptions options, IEventBus eventBus, IServiceProvider? serviceProvider = null) : this(options, serviceProvider)
    {
    }

    protected AbstractDbContextBase(DbContextOptions options, IServiceProvider? serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public IQueryable<T> SetReadOnly<T>() where T : class
    {
        return Set<T>().AsNoTracking();
    }

    public async Task RunInTransaction(Func<Task> action, List<int>? errorNumbersToRetry,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (Database.CurrentTransaction != null)
        {
            await action();
            return;
        }

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
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception)
                {
                    // Preserve the original error for the execution strategy/broker retry.
                }
                throw;
            }
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(_outbox, _outbox.Transactions);

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OutboxModel.Configure(modelBuilder, Database.ProviderName);

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
}
