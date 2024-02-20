﻿using System.Data;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database;

public class DevicesDbContext : IdentityDbContext<ApplicationUser>, IDevicesDbContext
{
    private readonly IServiceProvider? _serviceProvider;
    private const int MAX_RETRY_COUNT = 50000;
    private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromSeconds(1);
    private const string SQLSERVER = "Microsoft.EntityFrameworkCore.SqlServer";
    private const string POSTGRES = "Npgsql.EntityFrameworkCore.PostgreSQL";

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options)
        : base(options)
    {
    }

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options, IServiceProvider serviceProvider)
        : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public DbSet<Identity> Identities { get; set; } = null!;

    public DbSet<Device> Devices { get; set; } = null!;

    public DbSet<Challenge> Challenges { get; set; } = null!;

    public DbSet<Tier> Tiers { get; set; } = null!;

    public DbSet<PnsRegistration> PnsRegistrations { get; set; } = null!;

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

    public List<string> GetFcmAppIdsForWhichNoConfigurationExists(ICollection<string> supportedAppIds)
    {
        return GetAppIdsForWhichNoConfigurationExists("fcm", supportedAppIds);
    }

    public List<string> GetApnsBundleIdsForWhichNoConfigurationExists(ICollection<string> supportedAppIds)
    {
        return GetAppIdsForWhichNoConfigurationExists("apns", supportedAppIds);
    }

    private List<string> GetAppIdsForWhichNoConfigurationExists(string platform, ICollection<string> supportedAppIds)
    {
        var query = PnsRegistrations.FromSqlRaw(
            Database.IsNpgsql()
                ? $"""
                     SELECT "AppId"
                     FROM "Devices"."PnsRegistrations"
                     WHERE "Handle" LIKE '{platform}%'
                   """
                : $"""
                     SELECT "AppId"
                     FROM [Devices].[PnsRegistrations]
                     WHERE Handle LIKE '{platform}%'
                   """);

        return query
            .Where(x => !supportedAppIds.Contains(x.AppId))
            .Select(x => x.AppId)
            .Distinct()
            .ToList();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<IdentityAddress>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(IdentityAddress.MAX_LENGTH).HaveConversion<IdentityAddressValueConverter>();
        configurationBuilder.Properties<DeviceId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(DeviceId.MAX_LENGTH).HaveConversion<DeviceIdValueConverter>();
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

        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DeviceEntityTypeConfiguration).Assembly);
    }
}
