using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaEntityTypeConfiguration : IEntityTypeConfiguration<TierQuota>
{
    public void Configure(EntityTypeBuilder<TierQuota> builder)
    {
        builder.ToTable(nameof(TierQuota) + "s");
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.Weight);
        builder.HasOne("_definition").WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.Property("_definitionId").HasColumnName("DefinitionId");
        builder.Ignore(x => x.DefinitionId);
    }
}
