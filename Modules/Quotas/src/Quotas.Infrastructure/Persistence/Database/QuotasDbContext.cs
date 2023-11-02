using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Messages;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Aggregates.Tokens;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

public class QuotasDbContext : AbstractDbContextBase
{
    public QuotasDbContext() : base() { }

    public QuotasDbContext(DbContextOptions<QuotasDbContext> options) : base(options) { }

    public QuotasDbContext(DbContextOptions<QuotasDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider) { }

    public DbSet<Identity> Identities { get; set; }

    public DbSet<Tier> Tiers { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<FileMetadata> Files { get; set; }

    public DbSet<Relationship> Relationships { get; set; }

    public DbSet<RelationshipTemplate> RelationshipTemplates { get; set; }

    public DbSet<Token> Tokens { get; set; }

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
        configurationBuilder.Properties<TierId>().AreUnicode(false).AreFixedLength().HaveMaxLength(20)
            .HaveConversion<TierIdEntityFrameworkValueConverter>();

        configurationBuilder.Properties<MetricKey>().AreUnicode(true).AreFixedLength(false)
            .HaveMaxLength(50).HaveConversion<MetricKeyEntityFrameworkValueConverter>();
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeValueConverter>();
        configurationBuilder.Properties<ExhaustionDate>().HaveConversion<ExhaustionDateValueConverter>();
    }
}
