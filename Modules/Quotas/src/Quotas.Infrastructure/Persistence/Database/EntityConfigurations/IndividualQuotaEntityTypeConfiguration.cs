using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IndividualQuotaEntityTypeConfiguration : IEntityTypeConfiguration<IndividualQuota>
{
    public void Configure(EntityTypeBuilder<IndividualQuota> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
