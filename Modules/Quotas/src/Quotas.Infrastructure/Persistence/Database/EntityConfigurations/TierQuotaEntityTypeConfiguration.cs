using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaEntityTypeConfiguration : IEntityTypeConfiguration<TierQuota>
{
    public void Configure(EntityTypeBuilder<TierQuota> builder)
    {
        builder.UseTpcMappingStrategy();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApplyTo);
        builder.Property(x => x.IsExhaustedUntil);
        builder.HasOne("_definition");
    }
}
