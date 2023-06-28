using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class MetricStatusEntityTypeConfiguration : IEntityTypeConfiguration<MetricStatus>
{
    public void Configure(EntityTypeBuilder<MetricStatus> builder)
    {
        builder.HasKey(x => new { x.Owner, x.MetricKey });
        builder.HasOne<Identity>().WithMany().HasForeignKey(x => x.Owner);
    }
}
