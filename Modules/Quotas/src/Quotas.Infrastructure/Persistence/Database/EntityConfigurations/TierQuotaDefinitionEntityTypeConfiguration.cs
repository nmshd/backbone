using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<TierQuotaDefinition>
{
    public void Configure(EntityTypeBuilder<TierQuotaDefinition> builder)
    {
        builder.ToTable(nameof(TierQuotaDefinition) + "s");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Period);
        builder.Property(x => x.MetricKey);
        builder.Property(x => x.Max);
    }
}
