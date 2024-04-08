using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class MetricStatusEntityTypeConfiguration : IEntityTypeConfiguration<MetricStatus>
{
    public void Configure(EntityTypeBuilder<MetricStatus> builder)
    {
        builder.ToTable(nameof(MetricStatus) + "es");
        builder.HasKey(x => new { x.Owner, x.MetricKey });

        var indexBuilder = builder.HasIndex(x => x.MetricKey);
        NpgsqlIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.IsExhaustedUntil);
        SqlServerIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.IsExhaustedUntil);

        builder.HasOne<Identity>().WithMany(x => x.MetricStatuses).HasForeignKey(x => x.Owner);
    }
}
