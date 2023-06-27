using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class MetricStatusEntityTypeConfiguration : IEntityTypeConfiguration<MetricStatus>
{
    public void Configure(EntityTypeBuilder<MetricStatus> builder)
    {
        builder.HasAlternateKey(x => new { x.IdentityAddress, x.MetricKey });
    }
}
