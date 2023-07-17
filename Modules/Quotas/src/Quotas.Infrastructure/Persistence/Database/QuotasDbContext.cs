using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Aggregates.Messages;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

public class QuotasDbContext : AbstractDbContextBase
{
    public QuotasDbContext(DbContextOptions<QuotasDbContext> options) : base(options) { }

    public DbSet<Identity> Identities { get; set; }

    public DbSet<Tier> Tiers { get; set; }

    public DbSet<TierQuota> TierQuotas { get; set; }

    public DbSet<TierQuotaDefinition> TierQuotaDefinitions { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<FileMetadata> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(QuotasDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<QuotaId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(QuotaId.MAX_LENGTH).HaveConversion<QuotaIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<TierQuotaDefinitionId>().AreUnicode(false).AreFixedLength()
            .HaveMaxLength(TierQuotaDefinitionId.MAX_LENGTH).HaveConversion<TierQuotaDefinitionIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<TierId>().AreUnicode(false).AreFixedLength()
            .HaveConversion<TierIdEntityFrameworkValueConverter>();

        configurationBuilder.Properties<MetricKey>().AreUnicode(true).AreFixedLength(false)
            .HaveMaxLength(50).HaveConversion<MetricKeyEntityFrameworkValueConverter>();
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
        configurationBuilder.Properties<ExhaustionDate>().HaveConversion<ExhaustionDateValueConverter>();
    }
}