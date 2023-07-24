using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierQuotaEntityTypeConfiguration : IEntityTypeConfiguration<TierQuota>
{
    public void Configure(EntityTypeBuilder<TierQuota> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne("_definition");
        builder.Property("_definitionId").HasColumnName("DefinitionId");
        builder.Ignore(x => x.DefinitionId);
    }
}
