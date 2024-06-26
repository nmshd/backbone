using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaDefinitionEntityTypeConfiguration : EntityEntityTypeConfiguration<TierQuotaDefinition>
{
    public override void Configure(EntityTypeBuilder<TierQuotaDefinition> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.ToTable($"{nameof(TierQuotaDefinition)}s");
        builder.Property(x => x.Period);
        builder.Property(x => x.MetricKey);
        builder.Property(x => x.Max);
    }
}
