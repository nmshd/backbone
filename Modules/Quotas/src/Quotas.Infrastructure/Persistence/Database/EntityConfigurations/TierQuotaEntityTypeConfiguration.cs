using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaEntityTypeConfiguration : EntityEntityTypeConfiguration<TierQuota>
{
    public override void Configure(EntityTypeBuilder<TierQuota> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.ToTable($"{nameof(TierQuota)}s");
        builder.Ignore(x => x.Weight);
        builder.HasOne("Definition").WithMany().HasForeignKey(nameof(TierQuota.DefinitionId)).OnDelete(DeleteBehavior.Cascade);
    }
}
