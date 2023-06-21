using Enmeshed.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enmeshed.Common.Infrastructure.Persistence.EntityConfigurations;
public class MetricStatusEntityTypeConfiguration : IEntityTypeConfiguration<MetricStatus>
{
    public void Configure(EntityTypeBuilder<MetricStatus> builder)
    {
        builder.Property(x => x.MetricKey).HasColumnType("string");
    }
}
