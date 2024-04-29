using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
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
    public QuotasDbContext()
    {
    }

    public QuotasDbContext(DbContextOptions<QuotasDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public QuotasDbContext(DbContextOptions<QuotasDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public DbSet<Identity> Identities { get; set; } = null!;

    public DbSet<Tier> Tiers { get; set; } = null!;

    public DbSet<Message> Messages { get; set; } = null!;

    public DbSet<FileMetadata> Files { get; set; } = null!;

    public DbSet<Relationship> Relationships { get; set; } = null!;

    public DbSet<RelationshipTemplate> RelationshipTemplates { get; set; } = null!;

    public DbSet<Token> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Quotas");

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
