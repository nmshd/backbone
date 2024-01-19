using System.Data;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public class AbstractDbContextBase : DbContext, IDbContext
{
    private readonly IServiceProvider? _serviceProvider;
    private const int MAX_RETRY_COUNT = 50000;
    private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromSeconds(1);
    private const string SQLSERVER = "Microsoft.EntityFrameworkCore.SqlServer";
    private const string POSTGRES = "Npgsql.EntityFrameworkCore.PostgreSQL";

    protected AbstractDbContextBase()
    {
    }

    protected AbstractDbContextBase(DbContextOptions options, IServiceProvider? serviceProvider = null) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public IQueryable<T> SetReadOnly<T>() where T : class
    {
        return Set<T>().AsNoTracking();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (EnvironmentVariables.DEBUG_PERFORMANCE && _serviceProvider != null)
            optionsBuilder.AddInterceptors(_serviceProvider.GetRequiredService<SaveChangesTimeInterceptor>());
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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<IdentityAddress>().AreUnicode(false).AreFixedLength().HaveMaxLength(IdentityAddress.MAX_LENGTH).HaveConversion<IdentityAddressValueConverter>();
        configurationBuilder.Properties<DeviceId>().AreUnicode(false).AreFixedLength().HaveMaxLength(DeviceId.MAX_LENGTH).HaveConversion<DeviceIdValueConverter>();
        configurationBuilder.Properties<Username>().AreUnicode(false).AreFixedLength().HaveMaxLength(Username.MAX_LENGTH).HaveConversion<UsernameValueConverter>();

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
    }
}
