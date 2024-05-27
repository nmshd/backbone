using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IndividualQuotaEntityTypeConfiguration : EntityEntityTypeConfiguration<IndividualQuota>
{
    public override void Configure(EntityTypeBuilder<IndividualQuota> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.ToTable($"{nameof(IndividualQuota)}s");
        builder.Property(x => x.Period);
        builder.Property(x => x.MetricKey);
        builder.Property(x => x.Max);
        builder.Ignore(x => x.Weight);
    }
}
